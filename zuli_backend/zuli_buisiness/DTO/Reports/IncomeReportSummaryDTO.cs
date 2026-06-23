namespace zuli_Business.DTO.Reports;

public class IncomeReportSummaryDTO
{
    public int TotalFlights { get; set; }
    public int TotalFirstClass { get; set; }
    public int TotalTouristClass { get; set; }
    public int TotalPassengers { get; set; }
    public decimal TotalTicketIncome { get; set; }
    public decimal TotalBaggageIncome { get; set; }
    public decimal TotalIncome { get; set; }
}