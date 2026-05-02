using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AircraftController : ControllerBase
    {
        // Inyeccion de dependencias
        private readonly IAircraftService _service;
        public AircraftController(IAircraftService service) => _service = service;

        [HttpPost("CreateAircraft")]
        
        public async Task<ActionResult<BasicResponseDTO>> CreateAircraft([FromBody]AircraftDTO aircraft)
            => await _service.CreateAircraft(aircraft);

        [HttpGet("GetAll")]
        public async Task<IEnumerable<AircraftDTO>> GetAll()
            => await _service.GetAll();
    }
}
