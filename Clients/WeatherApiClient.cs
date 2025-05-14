
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain;
using Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace APIAggregator.Infrastructure
{
    public class WeatherApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<NewsApiClient>? logger = null) : IApiClient
    {
        private readonly ILogger<NewsApiClient> _logger = logger;

        private readonly HttpClient _httpClient = httpClient;

        public string ApiKey { get; set; } = configuration["WeatherApi:ApiKey"];
        public string ApiUrl { get; set; } = configuration["WeatherApi:ApiUrl"];

        public string ApiName { get; set; } = "OpenWeatherMap";

        public ClientCategory Category { get; set; } = ClientCategory.WeatherApi;


        public async Task<IEnumerable<AggregatedItemDto>> FetchAsync(CancellationToken cancellationToken, AggregatedDataDto data)
        {
            try { 

                var url = $"{ApiUrl}{data.Filter}&appid={ApiKey}&units=metric";


                var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var weather = JsonDocument.Parse(json);
                if (weather.RootElement.TryGetProperty("cod", out var cod) && cod.GetInt32() != 200)
                {
                    _logger?.LogWarning($"Error fetching data from {ApiName}: {weather.RootElement.GetProperty("message").GetString()}");
                }

                var result = new List<AggregatedItemDto>
            {
                new AggregatedItemDto
                {
                    Source = ApiName,
                    Title = $"Weather in {data.Filter}",
                    Timestamp = DateTime.UtcNow,
                    Description = weather.RootElement.GetProperty("weather")[0].GetProperty("description").GetString()
                }
            };
                _logger?.LogInformation($"weather data retrieved from {ApiName}, for {data.Filter} city");

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex.ToString());
                throw new Exception($"Error fetching data from {ApiName}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                throw;
            }

        }
    }
}
