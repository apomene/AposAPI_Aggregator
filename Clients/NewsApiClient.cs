using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain;
using Clients;
using Microsoft.Extensions.Configuration;
using System.Reflection;


namespace APIAggregator.Infrastructure
{
    public class NewsApiClient(HttpClient httpClient, IConfiguration configuration) : IApiClient
    {
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
                    Console.WriteLine($"Error {response.StatusCode}: {errorContent}");
                }

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
            catch (HttpRequestException ex)
            {
                //TO DO: Log
                throw new Exception($"Error fetching data from {ApiName}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                //TO DO: Log
                throw;
            }
        }
    }
}
