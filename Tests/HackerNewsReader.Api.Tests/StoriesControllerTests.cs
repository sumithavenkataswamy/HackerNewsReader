using HackerNewsReader.Api.Controllers;
using HackerNewsReader.Application.Interfaces;
using HackerNewsReader.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;

namespace HackerNewsReader.Api.Tests
{
    public class StoriesControllerTests
    {
        private readonly Mock<IStoryService> _storyServiceMock;
        private readonly StoriesController _controller;

        public StoriesControllerTests()
        {
            _storyServiceMock = new Mock<IStoryService>();
            _controller = new StoriesController(_storyServiceMock.Object);
        }

        [Fact]
        public async Task GetStories_ShouldReturnPagedStories_WhenStoriesExist()
        {
            // Arrange
            var pagedResult = new PagedResult<StoryDto>
            {
                Items = new List<StoryDto>
                {
                    new StoryDto { Title = "Story 1", Url = "http://example.com/story1" },
                    new StoryDto { Title = "Story 2", Url = "http://example.com/story2" }
                },
                TotalCount = 2
            };

            _storyServiceMock.Setup(s => s.GetPagedStoriesAsync(1, 2, null))
                             .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetStories(1, 2);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.ShouldNotBeNull();
            var response = okResult.Value as PagedResult<StoryDto>;
            response.ShouldNotBeNull();
            response.Items.Count.ShouldBe(2);
            response.TotalCount.ShouldBe(2);
        }

        [Fact]
        public async Task GetStories_ShouldReturnNotFound_WhenNoStoriesExist()
        {
            // Arrange
            var pagedResult = new PagedResult<StoryDto>
            {
                Items = new List<StoryDto>(),
                TotalCount = 0
            };

            _storyServiceMock.Setup(s => s.GetPagedStoriesAsync(1, 2, null))
                             .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetStories(1, 2);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.ShouldNotBeNull();
            notFoundResult.Value.ShouldBe("No stories found.");
        }

        [Fact]
        public async Task GetStories_ShouldReturnPagedStories_WhenSearchQueryIsProvided()
        {
            // Arrange
            var pagedResult = new PagedResult<StoryDto>
            {
                Items = new List<StoryDto>
                {
                    new StoryDto { Title = "Searched Story", Url = "http://example.com/story" }
                },
                TotalCount = 1
            };

            _storyServiceMock.Setup(s => s.GetPagedStoriesAsync(1, 10, "search"))
                             .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetStories(1, 10, "search");

            // Assert
            var okResult = result as OkObjectResult;
            okResult.ShouldNotBeNull();
            var response = okResult.Value as PagedResult<StoryDto>;
            response.ShouldNotBeNull();
            response.Items.Count.ShouldBe(1);
            response.TotalCount.ShouldBe(1);
            response.Items.First().Title.ShouldContain("Searched");
        }
    }
}
