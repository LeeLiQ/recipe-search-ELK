namespace recipe_search_ELK.Utilities
{
    public class ElasticConnectionSettings
    {
        private string _defaultIndex;
        public string ClusterUrl { get; set; }
        public string DefaultIndex
        {
            get
            {
                return _defaultIndex;
            }
            set
            {
                _defaultIndex = value.ToLower();
            }
        }
    }
}