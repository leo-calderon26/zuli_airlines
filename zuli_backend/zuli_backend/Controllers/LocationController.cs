using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _service;

        public LocationController(ILocationService service) => _service = service;

        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<CountryDTO>>> GetCountries()
        {
            return Ok(await _service.GetCountriesAsync());
        }

        [HttpGet("cities/{countryId}")]
        public async Task<ActionResult<IEnumerable<CityDTO>>> GetCities(int countryId)
        {
            return Ok(await _service.GetCitiesByCountryIdAsync(countryId));
        }
    }
}