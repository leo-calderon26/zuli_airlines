using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO.ReservationSearch;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaggageController : ControllerBase
    {
        private readonly IBaggageRegistrationService _service;

        public BaggageController(IBaggageRegistrationService service)
        {
            _service = service;
        }

        [HttpPost("additional")]
        public async Task<IActionResult> AddAdditionalBaggage([FromBody] AdditionalBaggageRequestDTO request)
        {
            await _service.AddAdditionalBaggageTransactional(
                request.ReservationCode,
                request.Passengers
            );

            return Ok(new
            {
                message = "Equipaje adicional agregado correctamente."
            });
        }
    }
}
