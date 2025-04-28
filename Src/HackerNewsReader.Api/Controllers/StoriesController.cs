using System.Net;
using HackerNewsReader.Application.Interfaces;
using HackerNewsReader.Application.Models;
using HackerNewsReader.Domain.Entities;
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

        /// <summary>
        /// Get paged stories with optional search query.
        /// </summary>
        /// <param name="page">Page number (default is 1).</param>
        /// <param name="pageSize">Page size (default is 10).</param>
        /// <param name="query">Optional search keyword.</param>
        /// <returns>Paged list of stories.</returns>
        [HttpGet]
        public async Task<IActionResult> GetStories([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? query = null)
        {
            try
            {
                var pagedStories = await _storyService.GetPagedStoriesAsync(page, pageSize, query);

                if (pagedStories == null || !pagedStories.Items.Any())
                {
                    var emptyResult = new PagedResult<Story>
                    {
                        Items = new List<Story>(),
                        TotalCount = 0
                    };
                    return Ok(emptyResult);
                }

                return Ok(pagedStories);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}
