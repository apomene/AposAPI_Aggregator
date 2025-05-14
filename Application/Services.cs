using APIAggregator.Infrastructure;
using Clients;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;


namespace Application
{
    public static class ApplicationServiceRegistration
    {
        //static HashSet<string> ApiNames { get; set; }
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAggregationService, AggregationService>();
            services.AddScoped<NewsApiClient>();
            services.AddScoped<WeatherApiClient>();
            services.AddScoped<IApiClientFactory, ApiClientFactory>();          
            return services;
        }
    }

    public class AggregationService : IAggregationService
    {

        private readonly ConcurrentBag<IApiClient> _apiClients = new ConcurrentBag<IApiClient>();
        private readonly IApiStatsTracker _statsTracker;


        public AggregationService(IApiClientFactory factory, IApiStatsTracker statsTracker)
        {
            var weatherClient = factory.CreateClient(ClientCategory.WeatherApi);
            var newsClient = factory.CreateClient(ClientCategory.NewsApi);

            _apiClients.Add(weatherClient);
            _apiClients.Add(newsClient);
            _statsTracker = statsTracker;
        }

        public async Task<IEnumerable<AggregatedItemDto>> GetAggregatedDataAsync(AggregatedDataDto aggregatedData, CancellationToken cancellationToken = default)
        {
            var relevantClients = _apiClients
                .Where(client => client.Category == aggregatedData.Category)
                .ToList();

            if (!relevantClients.Any())
            {
                throw new InvalidOperationException($"No clients found for category {aggregatedData.Category}");
            }

            var tasks = relevantClients.Select(async client =>
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    var result = await client.FetchAsync(cancellationToken, aggregatedData);
                    return result;
                }
                finally
                {
                    stopwatch.Stop();
                    _statsTracker.Record(client.ApiName, stopwatch.ElapsedMilliseconds);
                }
            });

            try
            {
                var results = await Task.WhenAll(tasks);
                return results.SelectMany(r => r);
            }
            catch (Exception)
            {               
                throw;
            }
        }

    }

}
