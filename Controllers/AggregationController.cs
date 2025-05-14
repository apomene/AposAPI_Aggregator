using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace AposAPI_Aggregator.Controllers
{
    /// <summary>
    /// Controller responsible for retrieving aggregated data from different external APIs.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AggregationController : ControllerBase
    {
        private readonly IAggregationService _aggregationService;
        private readonly ILogger<AggregationController> _logger;

        public AggregationController(IAggregationService aggregationService, ILogger<AggregationController> logger)
        {
            _aggregationService = aggregationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AggregatedDataDto data)
        {
            try
            {
                _logger.LogInformation("Received aggregation request: {Filter} / {Category}", data.Filter, data.Category);

                var result = await _aggregationService.GetAggregatedDataAsync(data);

                _logger.LogInformation("Aggregation successful. Items returned: {Count}", result.Count());

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get aggregated data for filter: {Filter}, category: {Category}", data.Filter, data.Category);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }

}
