using System.Text.Json.Serialization;

namespace Domain
{
    /// <summary>
    /// Represents a request for aggregated data from a specific API category.
    /// </summary>
    public class AggregatedDataDto
    {
        /// <summary>
        /// Filter criteria to apply to the results (e.g., keyword or topic).
        /// </summary>
        [JsonPropertyName("Filter")]
        public required string Filter { get; set; }

        /// <summary>
        /// Sorting option (e.g., 'date', 'relevance'). Optional.
        /// </summary>
        public string Sort { get; set; } = "";

        /// <summary>
        /// The category of API to query (e.g., WeatherApi, NewsApi, GitHub).
        /// </summary>
        [JsonRequired]
        [JsonPropertyName("api")]
        [JsonConverter(typeof(JsonStringEnumConverter))] // ✅ Converts enum to string in Swagger and JSON
        public ClientCategory Category { get; set; }
    }

    /// <summary>
    /// Defines the available API categories for aggregation.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))] // ✅ Converts enum values to string in responses
    public enum ClientCategory
    {
        WeatherApi,
        NewsApi,
        GitHub
    }

    /// <summary>
    /// Represents an aggregated item returned from an API client.
    /// </summary>
    public class AggregatedItemDto
    {
        /// <summary>
        /// Source of the data (e.g., API name).
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Title or heading of the aggregated item.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Timestamp when the data item was generated or retrieved.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Detailed description or content of the aggregated item.
        /// </summary>
        public string Description { get; set; }
    }
}
