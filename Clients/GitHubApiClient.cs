using System.Text.Json;
using System.Reflection;
using Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;


namespace Clients
{
    public class GitHubApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GitHubApiClient>? _logger;

        public string ApiUrl { get; set; } 
        public string ApiName { get; set; } = "GitHub";
        public ClientCategory Category { get; set; } = ClientCategory.GitHub;
        public string ApiKey { get; set; } = "";

        public GitHubApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<GitHubApiClient>? logger = null)
        {
            ApiKey = configuration["GitHubApi:ApiKey"];
            ApiUrl = configuration["GitHubApi:ApiUrl"];
            _httpClient = httpClient;
            _logger = logger;

            var appName = Assembly.GetEntryAssembly()?.GetName().Name ?? "API-Aggregator";
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(appName);
        }

        public async Task<IEnumerable<AggregatedItemDto>> FetchAsync(CancellationToken cancellationToken, AggregatedDataDto data)
        {
            try
            {
                var query = string.IsNullOrWhiteSpace(data.Filter) ? "dotnet" : data.Filter;
                var requestUrl = $"{ApiUrl}{query}&per_page=20";
               

                var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(content);

                var items = new List<AggregatedItemDto>();
                foreach (var repo in doc.RootElement.GetProperty("items").EnumerateArray())
                {
                    items.Add(new AggregatedItemDto
                    {
                        Source = ApiName,
                        Title = repo.GetProperty("full_name").GetString(),
                        Description = repo.GetProperty("description").GetString(),
                        Timestamp = DateTime.TryParse(repo.GetProperty("updated_at").GetString(), out var dt)
                                    ? dt : DateTime.UtcNow
                    });
                }

                _logger?.LogInformation($"{items.Count} GitHub repos fetched for '{query}'");       

                return items;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"GitHub API error: {ex.Message}");
                throw;
            }
        }
    }
}
