

namespace Domain
{
    public class ApiPerformanceStats
    {
        private int _requestCount;
        private double _totalResponseTimeMs;
        private int _fastCount;
        private int _averageCount;
        private int _slowCount;

        public int RequestCount => _requestCount;
        public double TotalResponseTimeMs => _totalResponseTimeMs;

        public double AverageResponseTimeMs => _requestCount == 0 ? 0 : _totalResponseTimeMs / _requestCount;

        public int FastCount => _fastCount;
        public int AverageCount => _averageCount;
        public int SlowCount => _slowCount;

        public void Record(long elapsedMs)
        {
            Interlocked.Increment(ref _requestCount);
            Interlocked.Exchange(ref _totalResponseTimeMs, _totalResponseTimeMs + elapsedMs);

            if (elapsedMs < 100)
                Interlocked.Increment(ref _fastCount);
            else if (elapsedMs <= 200)
                Interlocked.Increment(ref _averageCount);
            else
                Interlocked.Increment(ref _slowCount);
        }
    }


}
