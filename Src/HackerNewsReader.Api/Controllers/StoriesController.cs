using HackerNewsReader.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsReader.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly IStoryService _storyService;

        public StoriesController(IStoryService storyService)
        {
            _storyService = storyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStories([FromQuery] int page = 1, [FromQuery] int pageSize = 200)
        {
            var stories = await _storyService.GetStoriesAsync(page, pageSize);
            return Ok(stories);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStories([FromQuery] string query)
        {
            var result = await _storyService.SearchStoriesAsync(query);
            return Ok(result);
        }
    }
}
