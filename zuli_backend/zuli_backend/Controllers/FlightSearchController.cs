using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zuli_Buisiness.DTO;
using zuli_Buisiness.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightController(IFlightService flightService) => _flightService = flightService;

        [HttpGet("search")]
        public async Task<ActionResult<PagedFlightResponseDTO>> SearchFlights([FromQuery] FlightSearchRequestDTO request)
        {
            var result = await _flightService.Search(request);
            return Ok(result); 
        }
    }
}
