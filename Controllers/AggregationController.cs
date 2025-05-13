using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace AposAPI_Aggregator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AggregationController : ControllerBase
    {
        private readonly IAggregationService _aggregationService;

        public AggregationController(IAggregationService aggregationService)
        {
            _aggregationService = aggregationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AggregatedDataDto data, [FromQuery] string? sort = null)
        {
            var result = await _aggregationService.GetAggregatedDataAsync(data, sort);
            return Ok(result);
        }
    }

}
