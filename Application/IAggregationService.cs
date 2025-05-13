using Domain;


namespace Application
{
    public interface IAggregationService
    {
        Task<IEnumerable<AggregatedItemDto>> GetAggregatedDataAsync(string filter = null, string sort = null, CancellationToken cancellationToken = default);
    }

}
