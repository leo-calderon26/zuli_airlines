namespace zuli_Data.Entities.Reports
{
    public class IncomeReportRowEntity
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Flights { get; set; }
        public int FirstClass { get; set; }
        public int TouristClass  { get; set; }
        public int TotalPassengers { get; set; }
        public decimal TicketIncome { get; set; }
        public decimal BaggageIncome { get; set; }
        public decimal TotalIncome { get; set; }
    }

    public class IncomeReportSummaryEntity
    {
        public int TotalFlights { get; set; }
        public int TotalFirstClass { get; set; }
        public int TotalTouristClass { get; set; }
        public int TotalPassengers { get; set; }
        public decimal TotalTicketIncome { get; set; }
        public decimal TotalBaggageIncome { get; set; }
        public decimal TotalIncome { get; set; }
    }
        
    public class IncomeReportResultEntity
    {
        public List<IncomeReportRowEntity> Rows { get; set; }
        public IncomeReportSummaryEntity Summary { get; set; }
    }
}

