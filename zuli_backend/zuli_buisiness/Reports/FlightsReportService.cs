using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapsterMapper;
using zuli_Business.DTO.Reports;
using zuli_Business.DTO.Filters;
using zuli_Business.Interface;
using zuli_Data.Entities.Reports;
using zuli_Data.Entities.Filters;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class FlightsReportService : IFlightsReportService
    {
        private readonly IFlightsReportRepository _repository;
        private readonly IMapper _mapper;

        public FlightsReportService(IFlightsReportRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FlightsReportDTO>> GetFlightsReportAsync(FlightsReportFilterDTO filtersDTO)
        {
            var filterEntity = _mapper.Map<FlightsReportFilterEntity>(filtersDTO);

            var rawData = await _repository.GetFlightsReportAsync(filterEntity);

            var reportDTOs = _mapper.Map<IEnumerable<FlightsReportDTO>>(rawData);
            return reportDTOs;
        }
    }
}