using Application;
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
        public async Task<IActionResult> Get([FromQuery] string? filter = null, [FromQuery] string? sort = null)
        {
            var data = await _aggregationService.GetAggregatedDataAsync(filter, sort);
            return Ok(data);
        }
    }

}
