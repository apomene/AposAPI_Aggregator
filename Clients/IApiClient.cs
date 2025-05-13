using Domain;


namespace Clients
{
    public interface IApiClient
    {
        Task<IEnumerable<AggregatedItemDto>> FetchAsync(CancellationToken cancellationToken);
    }

}
