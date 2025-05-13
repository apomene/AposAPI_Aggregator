using APIAggregator.Infrastructure;
using Domain;
using Microsoft.Extensions.DependencyInjection;


namespace Clients
{
    public interface IApiClient
    {
        Task<IEnumerable<AggregatedItemDto>> FetchAsync(CancellationToken cancellationToken);
    }

    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IApiClient, WeatherApiClient>();
            //services.AddScoped<IWeatherApiClient, WeatherApiClient>();
            //services.AddScoped<INewsApiClient, NewsApiClient>();
            //services.AddScoped<IGitHubApiClient, GitHubApiClient>();
            //// Add Polly policies here if needed
            return services;
        }
    }


}
