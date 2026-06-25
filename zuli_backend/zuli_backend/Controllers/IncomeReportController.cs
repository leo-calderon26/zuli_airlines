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
        private readonly IIncomeReportExportService _exportService;
        public IncomeReportController(IIncomeReportService service, IIncomeReportExportService exportService)
        {
            _service = service;
            _exportService = exportService;
        } 
        [HttpGet]
        [Authorize(Roles = "Administrator,Operator")]
        public async Task<ActionResult<IncomeReportResultDTO>> GetIncome([FromQuery] IncomeReportRequestDTO request)
        {
            return Ok(await _service.GetIncomeReportAsync(request));
        }

        [HttpGet("export")]
        [Authorize(Roles = "Administrator,Operator")]
        public async Task<IActionResult> ExportIncome([FromQuery] IncomeReportRequestDTO request)
        {
            var stream = await _exportService.GenerateIncomeReportExcelAsync(request);

            var parts = new List<string> { "reporte_ingresos", request.Year.ToString() };
            if (!string.IsNullOrWhiteSpace(request.Origin)) parts.Add(request.Origin);
            if (!string.IsNullOrWhiteSpace(request.Destination)) parts.Add(request.Destination);
            if (request.AirlineId.HasValue) parts.Add($"airline_{request.AirlineId}");
            var filename = string.Join("_", parts) + ".xlsx";

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                filename);
        }
    }
}
