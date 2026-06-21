using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zuli_Business.DTO;
using zuli_Business.Interface;

namespace zuli_backend.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class FlightsReportController : ControllerBase
    {
        private readonly IFlightsReportService _service;

        public FlightsReportController(IFlightsReportService service) => _service = service;

        [HttpGet("GetFlightsReport")]
        public async Task<IEnumerable<FlightsReportDTO>> GetFlightsReport()
            => await _service.GetFlightsReportAsync();
    }
}