using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _service;
        public FlightController(IFlightService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookedFlightDTO>>> RetrieveAvailableFlights([FromQuery]RequestedFlightDTO requestedFlight)
            => Ok(await _service.RetrieveAvailableFlights(requestedFlight));

        [HttpPost("order")]
        public async Task<ActionResult<ReservationResponseDTO>> ReserveFlight([FromBody] ReservationRequestDTO reservationInfo)
            => Ok(await _service.ReserveFlight(reservationInfo));
    }
}
