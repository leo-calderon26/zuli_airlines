using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class FlightsReportService : IFlightsReportService
    {
        private readonly IFlightsReportRepository _repository;

        public FlightsReportService(IFlightsReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<FlightsReportDTO>> GetFlightsReportAsync()
        {

            var rawData = await _repository.GetFlightsReportAsync();
            var reportDTOs = rawData.Adapt<IEnumerable<FlightsReportDTO>>();

            return reportDTOs;
        }
    }
}