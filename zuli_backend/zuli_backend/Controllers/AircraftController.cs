using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zuli_Buisiness.DTO;
using zuli_Buisiness.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AircraftController : ControllerBase
    {
        // Inyeccion de dependencias
        private readonly IAircraftService _service;
        public AircraftController(IAircraftService service) => _service = service;

        [HttpPost]
        public async Task<ActionResult<BasicResponseDTO>> CreateAircraft(AircraftDTO aircraft)
            => await _service.CreateAircraft(aircraft);
    }
}
