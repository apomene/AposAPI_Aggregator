using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace AposAPI_Aggregator.Controllers
{     /// <summary>
      /// Controller responsible for retrieving aggregated data from different external APIs.
     /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AggregationController : ControllerBase
    {
        private readonly IAggregationService _aggregationService;

        public AggregationController(IAggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        /// <summary>
        /// Gets aggregated data from a selected API category.
        /// </summary>
        /// <param name="data">The filter, sort, and API category parameters.</param>
        /// <returns>A list of aggregated items.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AggregatedDataDto data)
        {
            var result = await _aggregationService.GetAggregatedDataAsync(data);
            return Ok(result);
        }
    }


}
