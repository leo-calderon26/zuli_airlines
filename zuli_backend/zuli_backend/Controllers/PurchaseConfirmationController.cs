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

        [HttpGet("{reservationCode}")]
        public async Task<ActionResult<PurchaseConfirmationPageDTO>> GetConfirmationPage(string reservationCode)
        {
            var confirmation = await _service.GetConfirmationPageAsync(reservationCode);
            return Ok(confirmation);
        }

        [HttpPost("Complete/{reservationCode}")]
        public async Task<ActionResult<PurchaseConfirmationPageDTO>> CompleteConfirmation(string reservationCode)
        {
            var confirmation = await _service.CompleteConfirmationAsync(reservationCode);
            return Ok(confirmation);
        }
    }
}