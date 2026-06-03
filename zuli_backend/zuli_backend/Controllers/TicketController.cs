using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketPurchaseService _service;
        public TicketController(ITicketPurchaseService service) => _service = service;

        [HttpPost("purchase")]
        public async Task<ActionResult<TicketPurchaseResponseDTO>> Purchase([FromBody] TicketPurchaseRequestDTO request)
        {
            var result = await _service.Purchase(request);
            return Ok(result);
        }
    }
}
