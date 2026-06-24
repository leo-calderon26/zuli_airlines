using zuli_Business.DTO.Reports;
namespace zuli_Business.Interface.Reports
{
    public interface IIncomeReportService
    {
        Task<IncomeReportResultDTO> GetIncomeReportAsync(IncomeReportRequestDTO incomeReportRequestDto);
    }    
}
