using HackerNewsReader.Application.Constants;
using HackerNewsReader.Application.Interfaces;
using HackerNewsReader.Application.Services;
using HackerNewsReader.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace HackerNewsReader.Application.Tests
{
    public class StoryServiceTests
    {
        private readonly Mock<IHackerNewsReaderService> _readerServiceMock;
        private readonly IMemoryCache _memoryCache;
        private readonly StoryService _storyService;

        public StoryServiceTests()
        {
            _readerServiceMock = new Mock<IHackerNewsReaderService>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _storyService = new StoryService(_readerServiceMock.Object, _memoryCache, Mock.Of<ILogger<StoryService>>());
        }

        [Fact]
        public async Task GetStoriesAsync_ShouldReturnPagedStories()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2 };
            var story = new Story { Title = "Story 1", Url = "http://example.com/story1" };
            var stories = new List<Story> { story };

            _readerServiceMock.Setup(s => s.GetNewStoryIdsAsync()).ReturnsAsync(storyIds);
            _readerServiceMock.Setup(s => s.GetStoryByIdAsync(1)).ReturnsAsync(story);
            _readerServiceMock.Setup(s => s.GetStoryByIdAsync(2)).ReturnsAsync((Story)null);

            // Act
            var result = await _storyService.GetStoriesAsync(1, 1);

            // Assert
            result.Items.Count.ShouldBeEquivalentTo(1);
            result.TotalCount.ShouldBeEquivalentTo(1);
        }

        [Fact]
        public async Task GetCachedStoriesAsync_ShouldCacheResults()
        {
            // Arrange
            var storyIds = new List<int> { 1 };
            var story = new Story { Title = "Story 1", Url = "http://example.com/story1" };

            _readerServiceMock.Setup(s => s.GetNewStoryIdsAsync()).ReturnsAsync(storyIds);
            _readerServiceMock.Setup(s => s.GetStoryByIdAsync(1)).ReturnsAsync(story);
            var cacheKey = "CachedStories_1_1";
            // Act
            var result = await _storyService.GetStoriesAsync(1, 1);

            // Assert
            _memoryCache.TryGetValue(cacheKey, out _).ShouldBeTrue();
        }
    }
}