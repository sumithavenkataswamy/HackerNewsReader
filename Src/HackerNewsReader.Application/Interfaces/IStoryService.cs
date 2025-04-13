using HackerNewsReader.Domain.Entities;

namespace HackerNewsReader.Application.Interfaces
{
    public interface IStoryService
    {
        Task<PagedResult<Story>> GetStoriesAsync(int page, int pageSize);
        Task<List<Story>> SearchStoriesAsync(string query);
    }
}
