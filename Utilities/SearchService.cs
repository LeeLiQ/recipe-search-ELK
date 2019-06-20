using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using recipe_search_ELK.Models;

namespace recipe_search_ELK.Utilities
{
    public class SearchService
    {
        private readonly ElasticClient _client;
        public SearchService(ElasticClientProvider clientProvider)
        {
            _client = clientProvider.Client;
        }

        /// <summary>
        /// Used to query results like garlic tomatoes^2 -egg* "chopped onions" -"red-pepper flakes" kind of query string
        /// which means include garlic/tomatoes/chopped onions as a whole/ and where tomatoes needs to have higher score but ignore anything
        /// starts with egg and red-pepper flakes.
        /// score means that keyword appears >= the number after ^ times.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<SearchResult<Recipe>> Search(string query, int page, int pageSize)
        {
            var response = await _client.SearchAsync<Recipe>(searchDescriptor => searchDescriptor
                    .Query(queryContainerDescriptor => queryContainerDescriptor
                        .Bool(queryDescriptor => queryDescriptor
                            .Must(queryStringQuery => queryStringQuery
                                .QueryString(queryString => queryString
                                    .Query(query)))))
                                        .From((page - 1) * pageSize)
                                        .Size(pageSize));
            return new SearchResult<Recipe>
            {
                Total = response.Total,
                ElapsedMilliseconds = response.Took,
                Page = page,
                PageSize = pageSize,
                Results = response.Documents
            };
        }

        public async Task<List<AutocompleteResult>> Autocomplete(string query)
        {
            var response = await _client.SearchAsync<Recipe>(sr => sr
                              .Suggest(scd => scd
                                  .Completion("recipe-name-completion", cs => cs
                                      .Prefix(query)
                                      .Fuzzy(fsd => fsd
                                          .Fuzziness(Fuzziness.Auto))
                                      .Field(r => r.Name))));

            var suggestions = ExtractAutocompleteSuggestions(response);
        }

        private List<AutocompleteResult> ExtractAutocompleteSuggestions(ISearchResponse<Recipe> response)
        {
            // var results = new List<AutocompleteResult>();

            // var suggestions = response.Suggest["recipe-name-completion"].Select(s => s.Options);

            // foreach (var suggestionsCollection in suggestions)
            // {
            //     foreach (var suggestion in suggestionsCollection)
            //     {
            //         var suggestedRecipe = suggestion.Source;

            //         var autocompleteResult = new AutocompleteResult
            //         {
            //             Id = suggestedRecipe.Id,
            //             Name = suggestedRecipe.Name
            //         };

            //         results.Add(autocompleteResult);
            //     }
            // }
            var matchingOptions = response.Suggest["recipe-name-completion"].Select(s => s.Options);
            var results = matchingOptions
                            .SelectMany(opt => opt
                                .Select(o => new AutocompleteResult
                                {
                                    Id = o.Source.Id,
                                    Name = o.Source.Name
                                }))
                            .ToList();
            return results;
        }
    }
}