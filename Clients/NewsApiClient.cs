using System.Text.Json;
using Domain;
using Clients;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.Logging;


namespace APIAggregator.Infrastructure
{
    public class NewsApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<NewsApiClient> ? logger = null) : IApiClient
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        private readonly ILogger<NewsApiClient> _logger = logger;
#pragma warning restore CS8601 // Possible null reference assignment.
        private readonly HttpClient _httpClient = httpClient;

        public string ApiKey { get; set; } = configuration["NewsApi:ApiKey"];
        public string ApiUrl { get; set; } = configuration["NewsApi:ApiUrl"];
        public string ApiName { get; set; } = "NewsAPI";

        public ClientCategory Category { get; set; } = ClientCategory.NewsApi;

        public async Task<IEnumerable<AggregatedItemDto>> FetchAsync(CancellationToken cancellationToken, AggregatedDataDto data)
        {
            try
            {
                var country = string.IsNullOrWhiteSpace(data.Filter) ? "us" : data.Filter;

                var requestUrl = $"{ApiUrl}{country}&apiKey={ApiKey}";
                var apiName = Assembly.GetEntryAssembly()?.GetName().Name;


                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(apiName);

                var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger?.LogError($"Error {response.StatusCode}: {errorContent}");
                }

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(content);

                var results = new List<AggregatedItemDto>();
                var articles = doc.RootElement.GetProperty("articles").EnumerateArray();
                if (articles.Any())
                {
                    foreach (var article in articles)
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
                    _logger?.LogInformation($"{articles.Count()} articles retrieved from {ApiName}");
                }
                else
                {
                     throw new HttpRequestException($"No articles found for filter: {data.Filter}");
                }
                return results;
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                throw;
            }
        }
    }
}
