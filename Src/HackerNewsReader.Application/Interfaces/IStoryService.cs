using HackerNewsReader.Application.Models;
namespace HackerNewsReader.Application.Interfaces
{
    public interface IStoryService
    {
        Task<PagedResult<StoryDto>> GetPagedStoriesAsync(int page, int pageSize, string? query = null);
    }
}
