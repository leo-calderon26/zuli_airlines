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
    public class ExternalFlightController : ControllerBase
    {
        // Inyeccion de dependencias
        private readonly IExternalFlightService _service;
        public ExternalFlightController(IExternalFlightService service) => _service = service;

        // Acordar con los demás grupos el status code utilizado
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RetrievedFlightDTO>>> RetrieveAvailableFlights([FromQuery]RequestedFlightDTO requestedFlight)
            => Ok(await _service.RetrieveAvailableFlights(requestedFlight));
    }
}
