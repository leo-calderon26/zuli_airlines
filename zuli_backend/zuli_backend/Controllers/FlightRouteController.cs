using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightRouteController : ControllerBase
    {
        private readonly IFlightRouteService _service;
        public FlightRouteController(IFlightRouteService service) => _service = service;

        [HttpPost("CreateFlightRouter")]
        public async Task<BasicResponseDTO> CreateFlightRouter([FromBody] FlightRouteDTO flightRouter)
            => await _service.CreateFlightRouter(flightRouter);
    }
}

