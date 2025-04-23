using HackerNewsReader.Application.Constants;
using HackerNewsReader.Application.Interfaces;
using HackerNewsReader.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace HackerNewsReader.Application.Services
{
    public class StoryService : IStoryService
    {
        private readonly IHackerNewsReaderService _readerService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<StoryService> _logger;

        public StoryService(
            IHackerNewsReaderService readerService,
            IMemoryCache cache,
            ILogger<StoryService> logger)
        {
            _readerService = readerService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<PagedResult<Story>> GetStoriesAsync(int page, int pageSize)
        {
            var stories = await GetCachedStoriesAsync();
            var pagedStories = stories.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<Story>
            {
                Items = pagedStories,
                TotalCount = stories.Count
            };
        }

        public async Task<List<Story>> SearchStoriesAsync(string query)
        {
            var stories = await GetCachedStoriesAsync();
            return stories
                .Where(story => story.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private async Task<List<Story>> GetCachedStoriesAsync()
        {
            if (_cache.TryGetValue(CacheKeys.StoryCacheKey, out List<Story> cachedStories))
            {
                return cachedStories;
            }

            var storyIds = await _readerService.GetNewStoryIdsAsync();
            var topStoryIds = storyIds.Take(200).ToList();
            var stories = new ConcurrentBag<Story>();

            await Parallel.ForEachAsync(topStoryIds, new ParallelOptions { MaxDegreeOfParallelism = 5 }, async (id, cancellationToken) =>
            {
                try
                {
                    var story = await _readerService.GetStoryByIdAsync(id);
                    if (story != null)
                    {
                        stories.Add(story);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching story with ID {StoryId}", id);
                }
            });

            var resultList = stories.ToList();
            _cache.Set(CacheKeys.StoryCacheKey, resultList, TimeSpan.FromMinutes(10));
            return resultList;
        }
    }
}
