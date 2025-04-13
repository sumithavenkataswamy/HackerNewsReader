using HackerNewsReader.Domain.Entities;

namespace HackerNewsReader.Domain.Tests
{
    public class PagedResultTests
    {
        [Fact]
        public void PagedResult_ShouldInitializeWithEmptyItems()
        {
            // Arrange & Act
            var pagedResult = new PagedResult<string>();

            // Assert
            Assert.NotNull(pagedResult.Items);
            Assert.Empty(pagedResult.Items);
            Assert.Equal(0, pagedResult.TotalCount);
        }

        [Fact]
        public void PagedResult_ShouldAllowSettingProperties()
        {
            // Arrange
            var pagedResult = new PagedResult<int>();

            // Act
            pagedResult.Items = new List<int> { 1, 2, 3 };
            pagedResult.TotalCount = 3;

            // Assert
            Assert.Equal(3, pagedResult.Items.Count);
            Assert.Equal(3, pagedResult.TotalCount);
        }
    }
}