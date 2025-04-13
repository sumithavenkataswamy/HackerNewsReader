namespace HackerNewsReader.Domain.Entities
{
    public class Story
    {
        public string By { get; set; } = string.Empty;
        public int Descendants { get; set; }
        public long Id { get; set; }
        public List<long> Kids { get; set; } = new();
        public int Score { get; set; }
        public long Time { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Url { get; set; }
    }
}
