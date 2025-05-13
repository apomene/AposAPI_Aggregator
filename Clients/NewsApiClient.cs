using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain;
using Clients;


namespace APIAggregator.Infrastructure
{
    public class NewsApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;

        public string ApiKey { get; set; }
        public string ApiUrl { get; set; } = "https://newsapi.org/v2/top-headlines?country=us&category=";
        public string ApiName { get; set; } = "NewsAPI";

        public NewsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<AggregatedItemDto>> FetchAsync(CancellationToken cancellationToken, AggregatedDataDto data)
        {
            try
            {
                var country = string.IsNullOrWhiteSpace(data.Filter) ? "us" : data.Filter;

                var requestUrl = $"{ApiUrl}{country}&apiKey={ApiKey}";

                var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(content);

                var results = new List<AggregatedItemDto>();

                foreach (var article in doc.RootElement.GetProperty("articles").EnumerateArray())
                {
                    results.Add(new AggregatedItemDto
                    {
                        Source = ApiName,
                        Title = article.GetProperty("title").GetString(),
                        Description = article.GetProperty("description").GetString(),
                        Timestamp = DateTime.TryParse(article.GetProperty("publishedAt").GetString(), out var parsedDate)
                                    ? parsedDate
                                    : DateTime.UtcNow
                    });
                }

                return results;
            }
            catch (Exception ex)
            {
                //TO DO: Log
                throw;
            }
        }
    }
}
