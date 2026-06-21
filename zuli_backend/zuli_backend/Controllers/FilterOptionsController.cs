using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO.Filters;
using zuli_Business.Interface;


namespace zuli_backend.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class FilterOptionsController : ControllerBase
    {
        private readonly IFilterOptionsService _optionsService; 
        public FilterOptionsController(IFilterOptionsService optionsService) 
            => _optionsService = optionsService;
        
        [HttpGet("filteroptions")]
        public async Task<ActionResult<FilterOptionsDTO>> GetFilterOptions()
            => Ok(await _optionsService.GetFilterOptionsAsync());
    }
}
