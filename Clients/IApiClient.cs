using APIAggregator.Infrastructure;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Clients
{
    public interface IApiClient
    {
        string ApiName { get; set; }
        string ApiKey { get; set; }
        string ApiUrl { get; set; }
        Task<IEnumerable<AggregatedItemDto>> FetchAsync(CancellationToken cancellationToken, AggregatedDataDto data);

        ClientCategory Category { get; set; }
    }

    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IApiClient, WeatherApiClient>();
            services.AddScoped<IApiClient, NewsApiClient>();
            services.AddScoped<IApiClient, GitHubApiClient>();
            return services;
        }
    }


}



