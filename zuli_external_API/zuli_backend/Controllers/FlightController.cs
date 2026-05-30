using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO.External;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FlightController : ControllerBase
    {
        // Inyeccion de dependencias
        private readonly IFlightService _service;
        public FlightController(IFlightService service) => _service = service;

        // Acordar con los demás grupos el status code utilizado
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookedFlightDTO>>> RetrieveAvailableFlights([FromQuery]RequestedFlightDTO requestedFlight)
            => Ok(await _service.RetrieveAvailableFlights(requestedFlight));
    }
}
