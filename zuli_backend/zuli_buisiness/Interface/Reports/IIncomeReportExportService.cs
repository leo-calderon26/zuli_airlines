using zuli_Business.DTO.Reports;

namespace zuli_Business.Interface.Reports
{
    public interface IIncomeReportExportService
    {
        Task<Stream> GenerateIncomeReportExcelAsync(IncomeReportRequestDTO request);
    }
}

