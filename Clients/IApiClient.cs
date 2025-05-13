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
    }

    public static class InfrastructureServiceRegistration
    {
        //    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        //    {
        //        services.AddScoped<IApiClient, WeatherApiClient>();
        //        services.AddScoped<IApiClient, NewsApiClient>();
        //        return services;
        //    }
        //}

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Weather client
            services.AddScoped<IApiClient, WeatherApiClient>();
            services.AddScoped<IApiClient, NewsApiClient>();
            services.AddScoped<IApiClient>(provider =>
            {
                var client = provider.GetRequiredService<WeatherApiClient>();
                client.ApiKey = configuration["WeatherApi:ApiKey"];
                return client;
            });

            // News client
            //services.AddHttpClient<NewsApiClient>();
            services.AddScoped<IApiClient>(provider =>
            {
                var client = provider.GetRequiredService<NewsApiClient>();
                client.ApiKey = configuration["NewsApi:ApiKey"];
                return client;
            });

            return services;
        }


    }


} 
