using System.Collections.Generic;

namespace recipe_search_ELK.Models
{
    public class SearchResult<T> where T : class
    {
        public long Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Results { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}