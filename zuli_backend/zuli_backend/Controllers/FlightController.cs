
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

        private readonly IFlightService _service;
        public FlightController(IFlightService service) => _service = service;

        [HttpPost("Create")]
        public async Task<ActionResult<BasicResponseDTO>> CreateFlight(AirportDTO Airport)
            => await _service.CreateFlight(Airport);
    }
}