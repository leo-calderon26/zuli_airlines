using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _service;
        public FlightController(IFlightService service) => _service = service;

        [HttpPost("Create")]
        public async Task<ActionResult<BasicResponseDTO>> CreateFlight([FromBody] FlightDTO flight)
            => await _service.CreateFlight(flight);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightDTO>>> GetAllFlights()
            => Ok(await _service.GetAllFlights());

        [HttpGet("{id}")]
        public async Task<ActionResult<FlightDTO>> GetFlightById(Guid id)
        {
            var flight = await _service.GetFlightById(id);
            if (flight == null)
                return NotFound(new { message = "Vuelo no encontrado" });
            return Ok(flight);
        }

        [HttpGet("search")]
        public async Task<ActionResult<FlightPaginatedResponseDTO>> SearchFlights([FromQuery] FlightSearchRequestDTO request)
        {
            var result = await _service.Search(request);
            return Ok(result);
        }
    }
}
