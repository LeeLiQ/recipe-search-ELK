using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using recipe_search_ELK.Utilities;

namespace recipe_search_ELK.Controllers
{
    [Route("api/[controller]")]
    public class IndexController : Controller
    {
        private readonly DataIndexer _indexer;
        public IndexController(DataIndexer indexer)
        {
            _indexer = indexer;
        }

        [HttpGet("file")]
        public async Task<IActionResult> IndexDataFromFile([FromQuery]string fileName, string index, bool deleteIndexIfExists)
        {
            var response = await _indexer.IndexRecipesFromFile(fileName, deleteIndexIfExists, index);
            return Ok(response);
        }
    }
}