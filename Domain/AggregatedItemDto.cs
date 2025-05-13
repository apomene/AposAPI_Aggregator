using System.Text.Json.Serialization;

namespace Domain
{
    public class AggregatedItemDto
    {
        public string Source { get; set; }
        public string Title { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }

        public ItemCategory Category { get; set; }
    }

    public enum ItemCategory
    {
        Weather,
        News,
        GitHub
    }

    public class AggregatedDataDto
    {
        //public List<AggregatedItemDto> Items { get; set; }
        [JsonPropertyName("Filter")]
        public required string Filter { get; set; }
        public string Sort { get; set; } = "";
    }   

}
