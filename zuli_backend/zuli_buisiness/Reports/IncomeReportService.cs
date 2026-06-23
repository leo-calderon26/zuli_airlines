using MapsterMapper;
using zuli_Business.DTO.Reports;
using zuli_Business.Interface.Reports;
using zuli_Data.Entities.Reports;
using zuli_Repository.Interface.Reports;

namespace zuli_Business.Reports
{
    public class IncomeReportService : IIncomeReportService
    {
        private readonly IIncomeReportRepository _reportRepository;
        private readonly IMapper _mapper;

        public IncomeReportService(IIncomeReportRepository reportRepository, IMapper mapper)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
        }

        public async Task<IncomeReportResultDTO> GetIncomeReportAsync(IncomeReportRequestDTO incomeReportRequestDto)
        {
            var result = await _reportRepository.GetIncomeReportAsync(
                _mapper.Map<IncomeReportRequestEntity>(incomeReportRequestDto));

            return _mapper.Map<IncomeReportResultDTO>(result);
        }
    }    
}

