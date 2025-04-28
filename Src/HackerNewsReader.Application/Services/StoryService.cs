using AutoMapper;
using HackerNewsReader.Application.Constants;
using HackerNewsReader.Application.Interfaces;
using HackerNewsReader.Application.Models;
using HackerNewsReader.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;

namespace HackerNewsReader.Application.Services
{
    public class StoryService : IStoryService
    {
        private readonly IHackerNewsReaderService _readerService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<StoryService> _logger;
        private readonly IMapper _mapper;

        public StoryService(
            IHackerNewsReaderService readerService,
            IMemoryCache cache,
            ILogger<StoryService> logger,
            IMapper mapper)
        {
            _readerService = readerService;
            _cache = cache;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a paged list of stories with optional title-based search filtering.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of stories per page.</param>
        /// <param name="query">Optional search keyword to filter stories by title.</param>
        /// <returns>Paged result containing a list of <see cref="StoryDto"/> and the total count.</returns>
        public async Task<PagedResult<StoryDto>> GetPagedStoriesAsync(int page, int pageSize, string? query = null)
        {
            // Fetch stories from cache or API
            var allStories = await GetStoriesFromCacheOrApiAsync();

            // Apply search filtering if query provided
            if (!string.IsNullOrWhiteSpace(query))
            {
                allStories = allStories.Where(story =>
                    !string.IsNullOrEmpty(story.Title) &&
                    story.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var totalCount = allStories.Count;

            // Apply pagination logic
            var pagedStories = allStories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<StoryDto>
            {
                Items = pagedStories,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Retrieves the list of latest stories either from cache or by fetching from HackerNews API.
        /// Caches the result for subsequent requests.
        /// </summary>
        /// <returns>List of <see cref="StoryDto"/>.</returns>
        private async Task<List<StoryDto>> GetStoriesFromCacheOrApiAsync()
        {
            // Check if stories are available in cache
            if (_cache.TryGetValue(CacheKeys.StoryCacheKey, out List<StoryDto> cachedStories))
            {
                return cachedStories;
            }

            // Fetch latest story IDs
            var storyIds = await _readerService.GetNewStoryIdsAsync();

            // Take top 200 story IDs only
            var topStoryIds = storyIds.Take(200).ToList();
            var stories = new ConcurrentBag<Story>();

            // Fetch each story details in parallel
            await Parallel.ForEachAsync(topStoryIds, new ParallelOptions { MaxDegreeOfParallelism = 5 }, async (storyId, cancellationToken) =>
            {
                try
                {
                    var story = await _readerService.GetStoryByIdAsync(storyId);

                    // Only add stories that have a non-empty URL
                    if (story != null && !string.IsNullOrWhiteSpace(story.Url))
                    {
                        stories.Add(story);
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    // Log HTTP-related issues
                    _logger.LogError(httpEx, "HTTP error occurred while fetching story with ID {StoryId}.", storyId);
                }
                catch (JsonException jsonEx)
                {
                    // Log JSON deserialization issues
                    _logger.LogError(jsonEx, "JSON error occurred while processing story with ID {StoryId}.", storyId);
                }
                catch (Exception ex)
                {
                    // Catch-all for any other errors
                    _logger.LogError(ex, "Unexpected error occurred while fetching story with ID {StoryId}.", storyId);
                }
            });


            // Map Story entities to StoryDto
            var storyDtoList = _mapper.Map<List<StoryDto>>(stories.ToList());

            // Store the result in cache
            _cache.Set(CacheKeys.StoryCacheKey, storyDtoList);

            return storyDtoList;
        }
    }
}
