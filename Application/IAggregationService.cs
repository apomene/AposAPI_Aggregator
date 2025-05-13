using Domain;


namespace Application
{

    public interface IAggregationService
    {
      
        Task<IEnumerable<AggregatedItemDto>> GetAggregatedDataAsync(AggregatedDataDto data, string sort = null, 
            CancellationToken cancellationToken = default);

    }

   

}
