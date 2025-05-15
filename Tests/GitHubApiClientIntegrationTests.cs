using Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Domain;
using APIAggregator.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests
{
    public class GitHubApiClientIntegrationTests
    {
        private GitHubApiClient _client;

        [SetUp]
        public void Setup()
        {
            var httpClient = new HttpClient();


            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _client = new GitHubApiClient(httpClient,configuration)
            {
                ApiKey = configuration["GitHubApi:ApiKey"],
                ApiUrl = configuration["GitHubApi:ApiUrl"],

            };

    
        }

        [Test]
        [TestCase("nuint")]
        [TestCase("java")]
        [TestCase("dotnet")]
        [TestCase("python")]
        public async Task FetchAsync_WithValidFilter_ReturnsRepositories(string filter)
        {
            // Arrange
            var dto = new AggregatedDataDto { Filter = filter };

            // Act
            var result = await _client.FetchAsync(CancellationToken.None, dto);

            // Assert
            Assert.That(result, Is.Not.Empty, "Expected result to contain news articles.");
            var list = new List<AggregatedItemDto>(result);
            Assert.That(list.Any());
            foreach (var repo in list)
            {
                Console.WriteLine($"[{repo.Timestamp}] {repo.Title}");
                Assert.That(!string.IsNullOrWhiteSpace(repo.Title));
                Assert.That(_client.ApiName, Is.EqualTo(repo.Source));
            }
        }

        [Test]
        public void FetchAsync_WithInvalidUrl_ThrowsException()
        {
            _client.ApiUrl = "https://invalid.github.com/search/repositories?q=";

            var dto = new AggregatedDataDto { Filter = "nunit" };

            Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await _client.FetchAsync(CancellationToken.None, dto);
            });
        }
    }
}
