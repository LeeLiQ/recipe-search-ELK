using System;
using Nest;
using Microsoft.Extensions.Options;

namespace recipe_search_ELK.Utilities
{
    public class ElasticClientProvider
    {
        public ElasticClientProvider(IOptions<ElasticConnectionSettings> settings)
        {
            ConnectionSettings connectionSettings = new ConnectionSettings(new Uri(settings.Value.ClusterUrl));

            connectionSettings.EnableDebugMode();

            if (settings.Value.DefaultIndex != null)
                connectionSettings.DefaultIndex(settings.Value.DefaultIndex);

            Client = new ElasticClient(connectionSettings);
        }

        public ElasticClient Client { get; }
    }
}