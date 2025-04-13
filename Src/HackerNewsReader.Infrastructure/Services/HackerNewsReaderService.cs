using HackerNewsReader.Application.Interfaces;
using HackerNewsReader.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HackerNewsReader.Infrastructure.Services
{
    public class HackerNewsReaderService : IHackerNewsReaderService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HackerNewsReaderService> _logger;

        public HackerNewsReaderService(HttpClient httpClient, ILogger<HackerNewsReaderService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<int>> GetNewStoryIdsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<int>>("newstories.json") ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new story IDs.");
                throw;
            }
        }

        public async Task<Story?> GetStoryByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Story?>($"item/{id}.json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching story with ID {StoryId}.", id);
                throw;
            }
        }
    }
}
