using zuli_Data.Entities.Reports;

namespace zuli_Repository.Interface.Reports
{
    public interface IIncomeReportRepository
    {
        Task<IncomeReportResultEntity> GetIncomeReportAsync(IncomeReportRequestEntity incomeReportRequestEntity);
    }
}

