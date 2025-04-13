using HackerNewsReader.Api.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace HackerNewsReader.Api.Tests
{
    public class ExceptionMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_ShouldHandleException()
        {
            // Arrange
            var mockRequestDelegate = new Mock<RequestDelegate>();
            mockRequestDelegate.Setup(rd => rd.Invoke(It.IsAny<HttpContext>())).Throws(new Exception("Test exception"));

            var loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            var middleware = new ExceptionMiddleware(loggerMock.Object);

            var context = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(context, mockRequestDelegate.Object);

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        }
    }
}