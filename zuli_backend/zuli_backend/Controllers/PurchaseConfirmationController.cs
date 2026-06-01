using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseConfirmationController : ControllerBase
    {
        private readonly IPurchaseConfirmationService _service;

        public PurchaseConfirmationController(IPurchaseConfirmationService service)
        {
            _service = service;
        }

        [HttpGet("{reservationId}")]
        public async Task<ActionResult<PurchaseConfirmationPageDTO>> GetConfirmationPage(int reservationId)
        {
            var confirmation = await _service.GetConfirmationPageAsync(reservationId);

            if (confirmation == null)
            {
                return NotFound(new
                {
                    message = "No se encontró la reserva indicada."
                });
            }

            return Ok(confirmation);
        }

        [HttpPost("Complete/{reservationId}")]
        public async Task<ActionResult<PurchaseConfirmationPageDTO>> CompleteConfirmation(int reservationId)
        {
            var confirmation = await _service.CompleteConfirmationAsync(reservationId);

            return Ok(confirmation);
        }
    }
}