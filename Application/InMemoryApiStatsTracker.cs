using Domain;
using System.Collections.Concurrent;


namespace Application
{
    public class InMemoryApiStatsTracker : IApiStatsTracker
    {
        private readonly ConcurrentDictionary<string, ApiPerformanceStats> _stats = new();

        public void Record(string apiName, long elapsedMs)
        {
            var stat = _stats.GetOrAdd(apiName, _ => new ApiPerformanceStats());
            stat.Record(elapsedMs);
        }

        public IDictionary<string, ApiPerformanceStats> GetAll() => _stats;
    }


}
