using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO;
using zuli_Business.DTO.Cancellation;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationCancellationController : ControllerBase
    {
        private readonly IReservationCancellationService _service;

        public ReservationCancellationController(IReservationCancellationService service)
        {
            _service = service;
        }

        [HttpPost("request")]
        public async Task<ActionResult<BasicResponseDTO>> RequestCancellation([FromBody] RequestCancellationRequestDTO request)
        {
            var result = await _service.RequestCancellationAsync(request);
            return Ok(result);
        }

        [HttpPost("confirm")]
        public async Task<ActionResult<BasicResponseDTO>> ConfirmCancellation([FromBody] ConfirmCancellationRequestDTO request)
        {
            var result = await _service.ConfirmCancellationAsync(request);
            return Ok(result);
        }
    }
}
