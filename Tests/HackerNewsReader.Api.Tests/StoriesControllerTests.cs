using HackerNewsReader.Api.Controllers;
using HackerNewsReader.Application.Interfaces;
using HackerNewsReader.Domain.Entities;
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
        public async Task GetStories_ShouldReturnPagedStories_WhenCalled()
        {
            // Arrange
            var pagedResult = new PagedResult<Story>
            {
                Items = new List<Story>
            {
                new Story { Title = "Story 1", Url = "http://example.com/story1" },
                new Story { Title = "Story 2", Url = "http://example.com/story2" }
            },
                TotalCount = 2
            };

            _storyServiceMock.Setup(s => s.GetStoriesAsync(1, 2))
                             .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetStories(1, 2);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.ShouldNotBeNull();
            var response = okResult.Value as PagedResult<Story>;
            response.ShouldNotBeNull();
            response.Items.Count().ShouldBeEquivalentTo(2);
        }

        [Fact]
        public async Task SearchStories_ShouldReturnFilteredStories_WhenCalled()
        {
            // Arrange
            var stories = new List<Story>
        {
            new Story { Title = "Story 1", Url = "http://example.com/story1" }
        };

            _storyServiceMock.Setup(s => s.SearchStoriesAsync("Story 1"))
                             .ReturnsAsync(stories);

            // Act
            var result = await _controller.SearchStories("Story 1");

            // Assert
            var okResult = result as OkObjectResult;
            okResult.ShouldNotBeNull();
            var response = okResult.Value as List<Story>;
            response.ShouldNotBeNull();
            response.Count().ShouldBeEquivalentTo(1);
        }
    }
}