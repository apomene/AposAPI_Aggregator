using Clients;
using Domain;
using Microsoft.Extensions.DependencyInjection;


namespace Application
{
    public static class ApplicationServiceRegistration
    {
        //static HashSet<string> ApiNames { get; set; }
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAggregationService, AggregationService>();
            return services;
        }
    }

    public class AggregationService : IAggregationService
    {
       
       
        private readonly IEnumerable<IApiClient> _apiClients;

        public AggregationService(IEnumerable<IApiClient> apiClients)
        {
            _apiClients = apiClients;
        }

        public async Task<IEnumerable<AggregatedItemDto>> GetAggregatedDataAsync(AggregatedDataDto aggregatedData, 
            CancellationToken cancellationToken = default)
        {
            var tasks = _apiClients.Select(client => client.FetchAsync(cancellationToken, aggregatedData));
            var results = await Task.WhenAll(tasks);

            var combined = results.SelectMany(x => x);

            return combined;
        }
     
    }


}
