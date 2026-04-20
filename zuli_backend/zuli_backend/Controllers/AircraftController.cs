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
        public async Task<ActionResult<AircraftDTO>> CreateAircraft(AircraftDTO aircraft)
        {
            var createAircraft = await _service.CreateAircraft(aircraft);
            return Ok(createAircraft);
        }
    }
}
