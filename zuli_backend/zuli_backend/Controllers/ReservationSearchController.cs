using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO.ReservationSearch;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationSearchController : ControllerBase
    {
        private readonly IReservationSearchService _service;

        public ReservationSearchController(IReservationSearchService service)
        {
            _service = service;
        }

        [HttpPost("search")]
        public async Task<ActionResult<ReservationSearchResponseDTO>> SearchReservation([FromBody] ReservationSearchRequestDTO request)
        {
            var result = await _service.GetReservationDetailsAsync(request);
            return Ok(result);
        }

        [HttpGet("itinerary")]
        public async Task<IActionResult> DownloadItinerary([FromQuery] string reservationCode, [FromQuery] string lastName)
        {
            var request = new ReservationSearchRequestDTO { ReservationCode = reservationCode, LastName = lastName };
            var (fileContents, contentType, fileName) = await _service.GenerateItineraryPdfAsync(request);
            
            return File(fileContents, contentType, fileName);
        }
    }
}