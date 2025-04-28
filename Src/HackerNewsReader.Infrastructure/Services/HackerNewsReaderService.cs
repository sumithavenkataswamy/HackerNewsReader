using HackerNewsReader.Application.Interfaces;
using HackerNewsReader.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

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
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP error occurred while fetching new story IDs.");
                throw;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON deserialization error while fetching new story IDs.");
                throw;
            }
        }

        public async Task<Story?> GetStoryByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Story?>($"item/{id}.json");
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP error occurred while fetching story with ID {StoryId}.", id);
                throw;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON deserialization error while fetching story with ID {StoryId}.", id);
                throw;
            }
        }
    }
}