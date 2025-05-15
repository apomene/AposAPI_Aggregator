using APIAggregator.Infrastructure;
using Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Clients
{
    public interface IApiClientFactory
    {
        IApiClient CreateClient(ClientCategory category);
    }

    public class ApiClientFactory : IApiClientFactory
    {
        private readonly IServiceProvider _provider;

        public ApiClientFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IApiClient CreateClient(ClientCategory category)
        {
            return category switch
            {
                ClientCategory.WeatherApi => _provider.GetRequiredService<WeatherApiClient>(),
                ClientCategory.NewsApi => _provider.GetRequiredService<NewsApiClient>(),
                ClientCategory.GitHub => _provider.GetRequiredService<GitHubApiClient>(),
                _ => throw new NotImplementedException($"No client registered for {category}")
            };
        }
    }

}
