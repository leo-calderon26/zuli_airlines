using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO.Reports;
using zuli_Business.Interface.Reports;

namespace zuli_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class IncomeReportController : ControllerBase
    {
        private readonly IIncomeReportService _service;
        public IncomeReportController(IIncomeReportService service) => _service = service;
        [HttpGet]
        [Authorize(Roles = "Administrator,Operator")]
        public async Task<ActionResult<IncomeReportResultDTO>> GetIncome([FromQuery] IncomeReportRequestDTO request)
        {
            return Ok(await _service.GetIncomeReportAsync(request));
        }
    }
}
