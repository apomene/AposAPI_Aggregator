using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace AposAPI_Aggregator.Controllers
{
    /// <summary>
    /// Aggregates data from multiple APIs (e.g., weather, news) based on category and filter.
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

        /// <summary>
        /// Retrieves aggregated data from external APIs based on category and filter.
        /// </summary>
        /// <param name="data">Query object containing filter, category, and sort options.</param>
        /// <remarks>
        /// Supported sort values:
        /// - date: sort by oldest first
        /// - date_desc: sort by newest first (default)
        /// - title: sort alphabetically A-Z
        /// - title_desc: sort alphabetically Z-A
        /// </remarks>
        /// <response code="200">Returns the aggregated and sorted data</response>
        /// <response code="400">Invalid parameters or request failure</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<AggregatedItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromQuery] AggregatedDataDto data)
        {
            try
            {
                _logger.LogInformation("Received aggregation request: {Filter} / {Category}", data.Filter, data.Category);

                var result = await _aggregationService.GetAggregatedDataAsync(data);

                _logger.LogInformation("Aggregation successful. Items returned: {Count}", result.Count());

                return Ok(result);
            }
            catch (HttpRequestException  ex)
            {
                _logger.LogError(ex, "Invalid request: {Filter} / {Category}", data.Filter, data.Category);
                
                return BadRequest($"Invalid request:{ex.HttpRequestError.ToString()}. Please check your input.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get aggregated data for filter: {Filter}, category: {Category}", data.Filter, data.Category);
                var errorMessage = ex.Message;
                if (errorMessage.Contains("404")) {

                    return NotFound($"No data found for the given filter: {data.Filter} in category: {data.Category}");
                }
                else if (errorMessage.Contains("400"))
                {

                    return BadRequest(errorMessage);               
                }
                return StatusCode(500, "An unexpected error occurred. Please try again later.");

            }
        }
    }

}
