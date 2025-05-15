using Domain;

namespace Application
{
    public interface IApiStatsTracker
    {
        void Record(string apiName, long elapsedMs);
        IDictionary<string, ApiPerformanceStats> GetAll();
    }

}
