namespace zuli_Business.DTO.Reports
{
    public class IncomeReportResultDTO
    {
        public List<IncomeReportRowDTO> Rows { get; set; } = [];
        public IncomeReportSummaryDTO Summary { get; set; } = new();
    }
}
