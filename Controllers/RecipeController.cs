using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using recipe_search_ELK.Utilities;

namespace recipe_search_ELK.Controllers
{
    [Route("api/[controller]")]
    public class RecipeController : Controller
    {
        private readonly SearchService _searchService;
        public RecipeController(SearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("search")]
        public async Task<JsonResult> Search([FromQuery]string query, int page = 1, int pageSize = 10)
        {
            var result = await _searchService.Search(query, page, pageSize);
            return Json(result);
        }

        [HttpGet("autocomplete")]
        public async Task<JsonResult> Autocomplete([FromQuery]string query)
        {
            var result = await _searchService.Autocomplete(query);
            return Json(result);
        }

        [HttpGet("morelikethis")]
        public async Task<JsonResult> MoreLikeTHis([FromQuery]string id, int page = 1, int pageSize = 10)
        {
            var result = await _searchService.MoreLikeThis(id, page, pageSize);
            return Json(result);
        }
    }
}