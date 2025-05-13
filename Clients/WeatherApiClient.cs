
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain;
using Clients;

namespace APIAggregator.Infrastructure
{
    public class WeatherApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;

        public WeatherApiClient(HttpClient httpClient)
        {
            
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<AggregatedItemDto>> FetchAsync(CancellationToken cancellationToken)
        {
            var city = "London"; // You can parameterize this later
            var apiKey = "f5e8a8a263a02e8f9305f4a0755498c3";
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var weather = JsonDocument.Parse(json);

            var result = new List<AggregatedItemDto>
            {
                new AggregatedItemDto
                {
                    Source = "OpenWeatherMap",
                    Title = $"Weather in {city}",
                    Timestamp = DateTime.UtcNow,
                    Description = weather.RootElement.GetProperty("weather")[0].GetProperty("description").GetString()
                }
            };

            return result;
        }
    }
}
