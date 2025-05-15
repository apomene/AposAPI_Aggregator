using APIAggregator.Infrastructure;
using Clients;
using Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;


namespace Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAggregationService, AggregationService>();
            services.AddScoped<NewsApiClient>();
            services.AddScoped<WeatherApiClient>();
            services.AddScoped<GitHubApiClient>();
            services.AddScoped<IApiClientFactory, ApiClientFactory>();          
            return services;
        }
    }

    public class AggregationService : IAggregationService
    {

        private readonly ConcurrentBag<IApiClient> _apiClients = new ConcurrentBag<IApiClient>();
        private readonly IApiStatsTracker _statsTracker;
        private readonly IMemoryCache _cache;
        private readonly int _cacheDurationMinutes;


        public AggregationService(IApiClientFactory factory, IApiStatsTracker statsTracker, 
            IMemoryCache cache, IOptions<CacheSettings> cacheOptions)
        {
            var weatherClient = factory.CreateClient(ClientCategory.WeatherApi);
            var newsClient = factory.CreateClient(ClientCategory.NewsApi);
            var gitHubClient = factory.CreateClient(ClientCategory.GitHub);

            _apiClients.Add(weatherClient);
            _apiClients.Add(newsClient);
            _apiClients.Add(gitHubClient);
            _statsTracker = statsTracker;
            _cache = cache;
            _cacheDurationMinutes = cacheOptions.Value.CacheIntervalInMinutes;
        }

        public async Task<IEnumerable<AggregatedItemDto>> GetAggregatedDataAsync(
    AggregatedDataDto aggregatedData,
    CancellationToken cancellationToken = default)
        {
            string cacheKey = $"agg:{aggregatedData.Category}:{aggregatedData.Filter}:{aggregatedData.Sort}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<AggregatedItemDto> cachedResult))
            {
                return cachedResult;
            }
            var relevantClients = _apiClients
                .Where(client => client.Category == aggregatedData.Category)
                .ToList();

            if (!relevantClients.Any())
            {
                throw new InvalidOperationException($"No clients found for category {aggregatedData.Category}");
            }

            var resultBag = new ConcurrentBag<AggregatedItemDto>();

            await Parallel.ForEachAsync(relevantClients, cancellationToken, async (client, ct) =>
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    var items = await client.FetchAsync(ct, aggregatedData);
                    foreach (var item in items)
                    {
                        resultBag.Add(item); 
                    }
                }
                finally
                {
                    stopwatch.Stop();
                    _statsTracker.Record(client.ApiName, stopwatch.ElapsedMilliseconds);
                }
            });

            _cache.Set(cacheKey, resultBag, TimeSpan.FromMinutes(_cacheDurationMinutes));
            // Fallback global sort
            return aggregatedData.Sort?.ToLower() switch
            {
                "title" => resultBag.OrderBy(r => r.Title),
                "title_desc" => resultBag.OrderByDescending(r => r.Title),
                "date" => resultBag.OrderBy(r => r.Timestamp),
                "date_desc" => resultBag.OrderByDescending(r => r.Timestamp),
                _ => resultBag.OrderByDescending(r => r.Timestamp) // default fallback
            };
        }

    }

}
