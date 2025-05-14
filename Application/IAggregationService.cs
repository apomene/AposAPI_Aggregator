using Domain;


namespace Application
{

    public interface IAggregationService
    {
        Task<IEnumerable<AggregatedItemDto>> GetAggregatedDataAsync(AggregatedDataDto data,
            CancellationToken cancellationToken = default);

    }   

}
