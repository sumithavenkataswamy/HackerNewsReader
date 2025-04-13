using HackerNewsReader.Domain.Entities;

namespace HackerNewsReader.Domain.Tests
{
    public class StoryTests
    {
        [Fact]
        public void Story_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var story = new Story();

            // Assert
            Assert.NotNull(story.By);
            Assert.Equal(string.Empty, story.By);
            Assert.Equal(0, story.Descendants);
            Assert.Equal(0, story.Id);
            Assert.NotNull(story.Kids);
            Assert.Empty(story.Kids);
            Assert.Equal(0, story.Score);
            Assert.Equal(0, story.Time);
            Assert.NotNull(story.Title);
            Assert.Equal(string.Empty, story.Title);
            Assert.NotNull(story.Type);
            Assert.Equal(string.Empty, story.Type);
            Assert.Null(story.Url);
        }

        [Fact]
        public void Story_ShouldAllowSettingProperties()
        {
            // Arrange
            var story = new Story();

            // Act
            story.By = "Author";
            story.Descendants = 5;
            story.Id = 12345;
            story.Kids = new List<long> { 1, 2, 3 };
            story.Score = 100;
            story.Time = 1744483780;
            story.Title = "Sample Story";
            story.Type = "story";
            story.Url = "http://example.com";

            // Assert
            Assert.Equal("Author", story.By);
            Assert.Equal(5, story.Descendants);
            Assert.Equal(12345, story.Id);
            Assert.Equal(3, story.Kids.Count);
            Assert.Equal(100, story.Score);
            Assert.Equal(1744483780, story.Time);
            Assert.Equal("Sample Story", story.Title);
            Assert.Equal("story", story.Type);
            Assert.Equal("http://example.com", story.Url);
        }
    }
}