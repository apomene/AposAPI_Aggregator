
using APIAggregator.Infrastructure;
using Clients;
using Domain;
using Microsoft.Extensions.Configuration;
using NUnit.Framework.Interfaces;

namespace IntegrationTests
{
    public class NewsApiClientIntegrationTests
    {
        private IApiClient _client;

        [SetUp]
        public void Setup()
        {
            var httpClient = new HttpClient();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _client = new NewsApiClient(httpClient, configuration)
            {
                ApiKey = configuration["NewsApi:ApiKey"],
                ApiUrl = configuration["NewsApi:ApiUrl"],

            };

            
        }

        [Test]
        [TestCase("technology")]
        [TestCase("sports")]
        [TestCase("health")]
        [TestCase("science")]
        [Explicit("Requires a real NewsAPI key and live internet connection.")]
        public async Task FetchAsync_ReturnsNewsArticles_ForValidTopic(string filter)
        {
            // Arrange
            var data = new AggregatedDataDto
            {
                Filter = filter
            };

            // Act
            var result = await _client.FetchAsync(CancellationToken.None, data);

            // Assert
            Assert.That(result, Is.Not.Empty, "Expected result to contain news articles.");
            var list = new List<AggregatedItemDto>(result);
            Assert.That(list.Any());
            foreach (var article in list)
            {
                Console.WriteLine($"[{article.Timestamp}] {article.Title}");
                Assert.That(!string.IsNullOrWhiteSpace(article.Title));
                Assert.That(_client.ApiName,Is.EqualTo( article.Source));
            }
        }

        [Test]       
        public void FetchAsync_ValidKeyButNoArticles_ThrowsHttpRequestException()
        {
            var dto = new AggregatedDataDto { Filter = "zz" }; // Invalid/rare country code

            Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await _client.FetchAsync(CancellationToken.None, dto);
            });
        }

        [Test]
        public void FetchAsync_TimeoutOrInvalidUrl_ThrowsHttpRequestException()
        {
            _client.ApiUrl = "https://invalid-url.org"; // force failure

            var dto = new AggregatedDataDto { Filter = "us" };

            Assert.ThrowsAsync<UriFormatException>(async () =>
            {
                await _client.FetchAsync(CancellationToken.None, dto);
            });
        }

        [Test]
        public void FetchAsync_InvalidApiKey_ThrowsHttpRequestException()
        {
            var dto = new AggregatedDataDto { Filter = "us" };
            _client.ApiKey = "InvalidKey";
            Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await _client.FetchAsync(CancellationToken.None, dto);
            });
        }

    }
}
