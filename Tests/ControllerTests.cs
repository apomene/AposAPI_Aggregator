using System.Net.Http.Json;
using Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;

namespace IntegrationTests
{
    public class AggregationControllerSortingTests
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var factory = new WebApplicationFactory<Program>(); 
            _client = factory.CreateClient();
        }

        [TearDown]
        public void Teardown()
        {
            if (_client != null)
                _client.Dispose();           
        }

        [Test]
        public async Task AggregationController_SortsByTitleAscending()
        {
            // Arrange
            var query = "/api/aggregation?filter=dotnet&category=github&sort=title";

            // Act
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var response = await _client.GetAsync(query);
            var items = await response.Content.ReadFromJsonAsync<List<AggregatedItemDto>>();

            // Assert
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(items, Is.Not.Null.And.Not.Empty);
            var titles = items.Select(i => i.Title).ToList();
            Assert.That(titles, Is.Ordered.Ascending);
        }

        [Test]
        public async Task AggregationController_SortsByDateDescending_ByDefault()
        {
            // Arrange (no sort given)
            var query = "/api/aggregation?filter=health&category=newsApi";

            // Act
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var response = await _client.GetAsync(query);
            var items = await response.Content.ReadFromJsonAsync<List<AggregatedItemDto>>();

            // Assert
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var timestamps = items.Select(i => i.Timestamp).ToList();
            Assert.That(timestamps, Is.Ordered.Descending);
        }
    }
}
