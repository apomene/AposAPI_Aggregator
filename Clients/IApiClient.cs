using APIAggregator.Infrastructure;
using Domain;
using Microsoft.Extensions.DependencyInjection;


namespace Clients
{
    public interface IApiClient
    {
        string ApiKey { get; set; }
        string ApiUrl { get; set; }
        Task<IEnumerable<AggregatedItemDto>> FetchAsync(CancellationToken cancellationToken, AggregatedDataDto data);
    }

    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IApiClient, WeatherApiClient>();
            services.AddScoped<IApiClient, NewsApiClient>();
            //services.AddScoped<IGitHubApiClient, GitHubApiClient>();
            //// Add Polly policies here if needed
            return services;
        }
    }


}
