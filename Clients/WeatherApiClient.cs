
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain;
using Clients;
using Microsoft.Extensions.Configuration;

namespace APIAggregator.Infrastructure
{
    public class WeatherApiClient(HttpClient httpClient, IConfiguration configuration) : IApiClient
    {
        private readonly HttpClient _httpClient = httpClient;

        public string ApiKey { get; set; } = configuration["WeatherApi:ApiKey"];
        public string ApiUrl { get; set; } = configuration["WeatherApi:ApiUrl"];

        public string ApiName { get; set; } = "OpenWeatherMap";

        public ClientCategory Category { get; set; } = ClientCategory.WeatherApi;


        private List<string> _cities = new List<string>
        {
            "London",
            "New York",
            "Tokyo"
        };

        public async Task<IEnumerable<AggregatedItemDto>> FetchAsync(CancellationToken cancellationToken, AggregatedDataDto data)
        {
            try
            {
                var city = _cities.FirstOrDefault(c => c == data.Filter);
                //city = "Athens"; // For testing purposes, you can remove this line later

                var url = $"{ApiUrl}{city}&appid={ApiKey}&units=metric";


                var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var weather = JsonDocument.Parse(json);

                var result = new List<AggregatedItemDto>
            {
                new AggregatedItemDto
                {
                    Source = ApiName,
                    Title = $"Weather in {city}",
                    Timestamp = DateTime.UtcNow,
                    Description = weather.RootElement.GetProperty("weather")[0].GetProperty("description").GetString()
                }
            };

                return result;
            }
            catch (Exception ex)
            {
                //TO DO: Log
                throw;
            }

        }
    }
}
