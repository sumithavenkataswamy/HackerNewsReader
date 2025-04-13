using HackerNewsReader.Domain.Entities;
using HackerNewsReader.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net.Http.Json;

namespace HackerNewsReader.Infrastructure.Tests
{
    public class HackerNewsReaderServiceTests
    {
        private readonly Mock<ILogger<HackerNewsReaderService>> _loggerMock;
        private HttpClient _httpClient;
        private HackerNewsReaderService _service;

        public HackerNewsReaderServiceTests()
        {
            _loggerMock = new Mock<ILogger<HackerNewsReaderService>>();
        }

        [Fact]
        public async Task GetNewStoryIdsAsync_ShouldReturnListOfIds_WhenApiResponseIsSuccessful()
        {
            // Arrange
            var expectedIds = new List<int> { 1, 2, 3 };

            var mockHttpMessageHandler = new MockHttpMessageHandler(request =>
            {
                if (request.RequestUri!.ToString().EndsWith("newstories.json"))
                {
                    return Task.FromResult(new HttpResponseMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = JsonContent.Create(expectedIds)
                    });
                }

                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                });
            });

            _httpClient = new HttpClient(mockHttpMessageHandler)
            {
                BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
            };

            _service = new HackerNewsReaderService(_httpClient, _loggerMock.Object);

            // Act
            var result = await _service.GetNewStoryIdsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedIds.Count, result.Count);
            Assert.Equal(expectedIds, result);
        }

        [Fact]
        public async Task GetNewStoryIdsAsync_ShouldLogError_WhenApiThrowsException()
        {
            // Arrange
            var mockHttpMessageHandler = new MockHttpMessageHandler(request =>
            {
                throw new HttpRequestException("Network error");
            });

            _httpClient = new HttpClient(mockHttpMessageHandler)
            {
                BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
            };

            _service = new HackerNewsReaderService(_httpClient, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetNewStoryIdsAsync());

            // Verify logging
            _loggerMock.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error fetching new story IDs.")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }


        [Fact]
        public async Task GetStoryByIdAsync_ShouldReturnStory_WhenApiResponseIsSuccessful()
        {
            // Arrange
            var expectedStory = new Story
            {
                Id = 1,
                Title = "Story Title",
                Url = "http://example.com",
                By = "Author",
                Time = 1744483780,
                Type = "story",
                Descendants = 10,
                Score = 100,
                Kids = new List<long> { 2, 3 }
            };

            var mockHttpMessageHandler = new MockHttpMessageHandler(request =>
            {
                if (request.RequestUri!.ToString().EndsWith("item/1.json"))
                {
                    return Task.FromResult(new HttpResponseMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = JsonContent.Create(expectedStory)
                    });
                }

                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                });
            });

            _httpClient = new HttpClient(mockHttpMessageHandler)
            {
                BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
            };

            _service = new HackerNewsReaderService(_httpClient, _loggerMock.Object);

            // Act
            var result = await _service.GetStoryByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedStory.Id, result.Id);
            Assert.Equal(expectedStory.Title, result.Title);
            Assert.Equal(expectedStory.Url, result.Url);
            Assert.Equal(expectedStory.By, result.By);
            Assert.Equal(expectedStory.Time, result.Time);
            Assert.Equal(expectedStory.Type, result.Type);
            Assert.Equal(expectedStory.Descendants, result.Descendants);
            Assert.Equal(expectedStory.Score, result.Score);
            Assert.Equal(expectedStory.Kids, result.Kids);
        }

        [Fact]
        public async Task GetStoryByIdAsync_ShouldLogError_WhenApiThrowsException()
        {
            // Arrange
            var mockHttpMessageHandler = new MockHttpMessageHandler(request =>
            {
                throw new HttpRequestException("Network error");
            });

            _httpClient = new HttpClient(mockHttpMessageHandler)
            {
                BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
            };

            _service = new HackerNewsReaderService(_httpClient, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetStoryByIdAsync(1));

            // Verify logger call
            _loggerMock.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error fetching story with ID 1")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

    }
}