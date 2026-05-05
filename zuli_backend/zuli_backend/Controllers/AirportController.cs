using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AirportController : ControllerBase
    {
        // Inyeccion de dependencias
        private readonly IAirportService _service;
        public AirportController(IAirportService service) => _service = service;

        [HttpPost]
        public async Task<ActionResult<BasicResponseDTO>> CreateAirport(AirportDTO Airport)
            => await _service.CreateAirport(Airport);
    }
}
