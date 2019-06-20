using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using recipe_search_ELK.Models;

namespace recipe_search_ELK.Utilities
{
    public class DataIndexer
    {
        private readonly ElasticClient _client;
        private readonly string _contentRootPath;
        private readonly string _defaultIndex;
        public DataIndexer(ElasticClientProvider clientProvider, IHostingEnvironment env, IOptions<ElasticConnectionSettings> settings)
        {
            _client = clientProvider.Client;
            // Path to read and index json data.
            _contentRootPath = Path.Combine(env.ContentRootPath, "data");
            _defaultIndex = settings.Value.DefaultIndex;
        }

        public async Task<bool> IndexRecipesFromFile(string fileName, bool deleteIndexIfExists, string index = null)
        {
            index = index == null ? _defaultIndex : index.ToLower();

            if (_client.IndexExists(index).Exists && deleteIndexIfExists)
                await _client.DeleteIndexAsync(index);

            if (!_client.IndexExists(index).Exists)
            {
                var indexDescriptor = new CreateIndexDescriptor(index)
                                        .Mappings(mappings => mappings
                                        .Map<Recipe>(m => m.AutoMap()));
                await _client.CreateIndexAsync(index, i => indexDescriptor);
            }

            _client.UpdateIndexSettings(index, idx => idx
                                            .IndexSettings(s => s
                                            .Setting("max_result_window", int.MaxValue)));

            string rawJsonCollection = null;

            using (var fs = new FileStream(Path.Combine(_contentRootPath, fileName), FileMode.Open))
            {
                using (var reader = new StreamReader(fs))
                {
                    rawJsonCollection = await reader.ReadToEndAsync();
                }
            }

            Recipe[] mappedCollection = JsonConvert.DeserializeObject<Recipe[]>(rawJsonCollection, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });

            var batchSize = 10000;
            var totalBatches = (int)Math.Ceiling((double)mappedCollection.Length / batchSize);

            for (int i = 0; i < totalBatches; i++)
            {
                var response = await _client.IndexManyAsync(mappedCollection.Skip(i * batchSize).Take(batchSize));
                if (!response.IsValid)
                    return false;
            }
            return true;
        }
        private void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            var currentError = e.ErrorContext.Error.Message;
            e.ErrorContext.Handled = true;
        }

    }
}