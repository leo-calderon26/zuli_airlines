using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO.Reports;
using zuli_Business.DTO.Filters;
using zuli_Business.Interface;
using zuli_Business.Interface.Reports;

namespace zuli_backend.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class FlightsReportController : ControllerBase
    {
        private readonly IFlightsReportService _service;
        private readonly IFlightsReportExportService _exportService;

        public FlightsReportController(
            IFlightsReportService service,
            IFlightsReportExportService exportService)
        {
            _service = service;
            _exportService = exportService;
        }

        [HttpGet("GetFlightsReport")]
        [Authorize(Roles = "Administrator,Operator")]
        public async Task<IEnumerable<FlightsReportDTO>> GetFlightsReport([FromQuery] FlightsReportFilterDTO filters)
            => await _service.GetFlightsReportAsync(filters);

        [HttpGet("export")]
        [Authorize(Roles = "Administrator,Operator")]
        public async Task<IActionResult> ExportFlightsReport([FromQuery] FlightsReportFilterDTO filters)
        {
            var stream = await _exportService.GenerateFlightsReportExcelAsync(filters);

            var parts = new List<string> { "reporte_vuelos" };

            if (filters.FromDate.HasValue)
                parts.Add(filters.FromDate.Value.ToString("yyyyMMdd"));
            if (!string.IsNullOrWhiteSpace(filters.Origin))
                parts.Add(filters.Origin);
            if (!string.IsNullOrWhiteSpace(filters.Destination))
                parts.Add(filters.Destination);
            if (!string.IsNullOrWhiteSpace(filters.FlightClass))
                parts.Add(filters.FlightClass);

            string fileName = string.Join("_", parts) + ".xlsx";

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }
    }
}