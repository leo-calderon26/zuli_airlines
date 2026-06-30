using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class FlightRouteController : ControllerBase
    {
        private readonly IFlightRouteService _service;
        public FlightRouteController(IFlightRouteService service) => _service = service;

        [HttpPost("CreateFlightRoute")]
        public async Task<BasicResponseDTO> CreateFlightRouter([FromBody] FlightRouteDTO flightRouter)
            => await _service.CreateFlightRouter(flightRouter);

        [HttpGet("GetPaginated")]
        public async Task<FlightRoutePaginatedResponseDTO> GetPaginated(int pageNumber = 1, int pageSize = 10)
            => await _service.GetFlightRoutesPaginated(pageNumber, pageSize);
            
        [HttpDelete("DeleteFlightRoute/{flightRouteId:int}")]
        public async Task<BasicResponseDTO> DeleteFlightRoute(int flightRouteId)
            => await _service.DeleteFlightRoute(flightRouteId);
    }
}

