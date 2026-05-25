using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/admin/[controller]")]
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

        [HttpGet("GetPaginated")]
        public async Task<AircraftPaginatedResponseDTO<AircraftDTO>> GetPaginated(int pageNumber = 1, int pageSize = 10)
            => await _service.GetAircraftsPaginated(pageNumber, pageSize);

        [HttpPut("{aircraftId:guid}")]
        public async Task<ActionResult<BasicResponseDTO>> Update(Guid aircraftId, [FromBody] AircraftDTO aircraft)
            => await _service.UpdateAircraft(aircraftId, aircraft);
    }
}
