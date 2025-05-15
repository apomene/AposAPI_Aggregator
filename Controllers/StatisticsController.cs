using Application;
using Microsoft.AspNetCore.Mvc;

namespace AposAPI_Aggregator.Controllers
{
    /// <summary>
    /// Provides request performance statistics for each external API integrated with the aggregator.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IApiStatsTracker _tracker;

        public StatisticsController(IApiStatsTracker tracker)
        {
            _tracker = tracker;
        }

        /// <summary>
        /// Retrieves request statistics for each API, including total requests, average response time,
        /// and categorized performance buckets (fast, average, slow).
        /// </summary>
        /// <returns>A collection of performance statistics grouped by API name.</returns>
        /// <response code="200">Returns the performance statistics data.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
