using HackerNewsReader.Application.Interfaces;
using HackerNewsReader.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using HackerNewsReader.Application.Constants;
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
            var stories = await GetCachedStoriesAsync(page, pageSize);
            return new PagedResult<Story>
            {
                Items = stories,
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

        private async Task<List<Story>> GetCachedStoriesAsync(int page, int pageSize)
        {
            var cacheKey = $"{CacheKeys.StoryCacheKey}_{page}_{pageSize}";
            if (_cache.TryGetValue(cacheKey, out List<Story> cachedStories))
            {
                return cachedStories;
            }

            var storyIds = await _readerService.GetNewStoryIdsAsync();
            var pagedIds = storyIds.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var stories = new ConcurrentBag<Story>();

            await Parallel.ForEachAsync(pagedIds, new ParallelOptions { MaxDegreeOfParallelism = 5 }, async (id, cancellationToken) =>
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
            _cache.Set(cacheKey, resultList, TimeSpan.FromMinutes(10));
            return resultList;
        }


        private async Task<List<Story>> GetCachedStoriesAsync()
        {
            if (_cache.TryGetValue(CacheKeys.StoryCacheKey, out List<Story> cachedStories))
            {
                return cachedStories;
            }

            var storyIds = await _readerService.GetNewStoryIdsAsync();
            var stories = new ConcurrentBag<Story>();

            await Parallel.ForEachAsync(storyIds, new ParallelOptions { MaxDegreeOfParallelism = 5 }, async (id, cancellationToken) =>
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
