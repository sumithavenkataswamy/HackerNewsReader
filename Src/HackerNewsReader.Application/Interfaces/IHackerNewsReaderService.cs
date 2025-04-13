using HackerNewsReader.Domain.Entities;

namespace HackerNewsReader.Application.Interfaces
{
    public interface IHackerNewsReaderService
    {
        Task<List<int>> GetNewStoryIdsAsync();
        Task<Story?> GetStoryByIdAsync(int id);
    }
}
