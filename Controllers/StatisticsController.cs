using Application;
using Microsoft.AspNetCore.Mvc;

namespace AposAPI_Aggregator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IApiStatsTracker _tracker;

        public StatisticsController(IApiStatsTracker tracker)
        {
            _tracker = tracker;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var stats = _tracker.GetAll()
                .Select(kvp => new
                {
                    Api = kvp.Key,
                    kvp.Value.RequestCount,
                    AverageResponseTimeMs = kvp.Value.AverageResponseTimeMs,
                    Buckets = new
                    {
                        Fast = kvp.Value.FastCount,
                        Average = kvp.Value.AverageCount,
                        Slow = kvp.Value.SlowCount
                    }
                });

            return Ok(stats);
        }
    }

}
