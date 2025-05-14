using Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
