using APIAggregator.Infrastructure;
using Clients;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
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

        public AggregationService(IApiClientFactory factory)
        {
            var weatherClient = factory.CreateClient(ClientCategory.WeatherApi);
            var newsClient = factory.CreateClient(ClientCategory.NewsApi);

            _apiClients.Add(weatherClient);
            _apiClients.Add(newsClient);
        }

        //public AggregationService(IEnumerable<IApiClient> apiClients)
        //{ 

        //    foreach (var client in apiClients)
        //    {
                
        //            _apiClients.Add(client);
               
        //    }
        //}

        public async Task<IEnumerable<AggregatedItemDto>> GetAggregatedDataAsync( AggregatedDataDto aggregatedData, CancellationToken cancellationToken = default)
        {
            var relevantClients = _apiClients
                .Where(client => client.Category == aggregatedData.Category)
                .ToList();

            if (!relevantClients.Any())
            {
                throw new InvalidOperationException($"No clients found for category {aggregatedData.Category}");
            }

            var tasks = relevantClients
                .Select(client => client.FetchAsync(cancellationToken, aggregatedData));

            var results = await Task.WhenAll(tasks);

            return results.SelectMany(x => x);
        }

    }

}
