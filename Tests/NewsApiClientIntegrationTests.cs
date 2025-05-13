
using APIAggregator.Infrastructure;
using Domain;

namespace IntegrationTests
{
    public class NewsApiClientIntegrationTests
    {
        private NewsApiClient _client;

        [SetUp]
        public void Setup()
        {
            var httpClient = new HttpClient();

            _client = new NewsApiClient(httpClient)
            {
                ApiKey = "65ce4f3b539a4484b8e78ec46dcfe61f",
                ApiUrl = "https://newsapi.org/v2/top-headlines?country=us&category=",
                ApiName = "NewsAPI"
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
    }
}
