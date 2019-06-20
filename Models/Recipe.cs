using System;
using Nest;

namespace recipe_search_ELK.Models
{
    public class Recipe
    {
        public string Id { get; set; }
        // This is for Autocomplete purpose.
        [Completion]
        public string Name { get; set; }
        // This will enable full-text search
        [Text]
        public string Ingredients { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string CookTime { get; set; }
        public string RecipeYield { get; set; }
        public DateTime? DatePublished { get; set; }
        public string PrepTime { get; set; }

        [Text]
        public string Description { get; set; }
    }
}