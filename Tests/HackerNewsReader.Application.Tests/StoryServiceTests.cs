using AutoMapper;
using HackerNewsReader.Application.Constants;
using HackerNewsReader.Application.Interfaces;
using HackerNewsReader.Application.Models;
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
        private readonly Mock<IMapper> _mapperMock;
        private readonly StoryService _storyService;

        public StoryServiceTests()
        {
            _readerServiceMock = new Mock<IHackerNewsReaderService>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _mapperMock = new Mock<IMapper>();

            _storyService = new StoryService(
                _readerServiceMock.Object,
                _memoryCache,
                Mock.Of<ILogger<StoryService>>(),
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetPagedStoriesAsync_ShouldReturnPagedStories()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2 };
            var story = new Story { Title = "Story 1", Url = "http://example.com/story1" };
            var stories = new List<Story> { story };

            _readerServiceMock.Setup(s => s.GetNewStoryIdsAsync())
                              .ReturnsAsync(storyIds);

            _readerServiceMock.Setup(s => s.GetStoryByIdAsync(1))
                              .ReturnsAsync(story);

            _readerServiceMock.Setup(s => s.GetStoryByIdAsync(2))
                              .ReturnsAsync((Story)null);

            var storyDtos = new List<StoryDto>
            {
                new StoryDto { Title = "Story 1", Url = "http://example.com/story1" }
            };

            _mapperMock.Setup(m => m.Map<List<StoryDto>>(It.IsAny<IEnumerable<Story>>()))
                       .Returns(storyDtos);

            // Act
            var result = await _storyService.GetPagedStoriesAsync(1, 1);

            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.TotalCount.ShouldBe(1);
            result.Items.First().Title.ShouldBe("Story 1");
        }

        [Fact]
        public async Task GetPagedStoriesAsync_ShouldUseCache_WhenStoriesAreCached()
        {
            // Arrange
            var cachedStories = new List<StoryDto>
            {
                new StoryDto { Title = "Cached Story", Url = "http://example.com/cached" }
            };

            _memoryCache.Set(CacheKeys.StoryCacheKey, cachedStories);

            // Act
            var result = await _storyService.GetPagedStoriesAsync(1, 1);

            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.TotalCount.ShouldBe(1);
            result.Items.First().Title.ShouldBe("Cached Story");
        }

        [Fact]
        public async Task GetPagedStoriesAsync_ShouldApplySearchQuery_WhenProvided()
        {
            // Arrange
            var cachedStories = new List<StoryDto>
            {
                new StoryDto { Title = "First Story", Url = "http://example.com/first" },
                new StoryDto { Title = "Second Story", Url = "http://example.com/second" }
            };

            _memoryCache.Set(CacheKeys.StoryCacheKey, cachedStories);

            // Act
            var result = await _storyService.GetPagedStoriesAsync(1, 10, "Second");

            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.TotalCount.ShouldBe(1);
            result.Items.First().Title.ShouldBe("Second Story");
        }
    }
}
