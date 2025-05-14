
using Domain;
using APIAggregator.Infrastructure;
using Microsoft.Extensions.Configuration;
using Clients;


namespace APIAggregator.Tests.Integration
{
    public class WeatherApiClientIntegrationTests
    {
        private IApiClient _client;

        [SetUp]
        public void Setup()
        {
            var httpClient = new HttpClient();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) 
                .Build();

            _client = new WeatherApiClient(httpClient, configuration)
            {
                ApiKey = configuration["WeatherApi:ApiKey"],
                ApiUrl = configuration["WeatherApi:ApiUrl"]
            };
        }

        [Test]
        [TestCase("New York")]
        [TestCase("London")]
        [TestCase("Tokyo")]
        [Explicit("Runs against the real OpenWeatherMap API. Requires valid API key.")]
        public async Task FetchAsync_ReturnsWeatherData_FromRealApi(string filter)
        {
            // Arrange
            var data = new AggregatedDataDto
            {
                Filter = filter
            };

            // Act
            var result = await _client.FetchAsync(CancellationToken.None, data);

            // Assert
            Assert.That(result, Has.Some.Matches<AggregatedItemDto>(x => x.Title.Contains(filter)),
                $"Expected result to contain weather data for {filter}.");
            Assert.That(result, Has.Some.Matches<AggregatedItemDto>(x => x.Source.Contains(_client.ApiName)),
               message: $"Expected result, is from  {_client.ApiName}");
        }

        [Test]
        public void FetchAsync_InvalidApiKey_ThrowsException()
        {
            _client.ApiKey = "YOUR_VALID_API_KEY";
            var dto = new AggregatedDataDto { Filter = "London" };

            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _client.FetchAsync(CancellationToken.None, dto);
            });

            Assert.That(ex.Message, Does.Contain("Error fetching data from"));
        }

        [Test]
        [Explicit("Requires real API key and a fake city")]
        public void FetchAsync_InvalidCity_ThrowsException()
        {
           
            var dto = new AggregatedDataDto { Filter = "FakeCity12345" };

            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _client.FetchAsync(CancellationToken.None, dto);
            });

            Assert.That(ex.Message, Does.Contain("Error fetching data"));
        }

        [Test]
        public void FetchAsync_InvalidUrl_ThrowsHttpRequestException()
        {
            _client.ApiUrl = "https://invalid.openweathermap.org/api?q=";

            var dto = new AggregatedDataDto { Filter = "London" };

            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _client.FetchAsync(CancellationToken.None, dto);
            });

            Assert.That(ex.InnerException, Is.TypeOf<HttpRequestException>());
        }
    }
}
    

