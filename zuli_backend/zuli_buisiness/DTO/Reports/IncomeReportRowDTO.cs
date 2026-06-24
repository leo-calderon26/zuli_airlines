namespace zuli_Business.DTO.Reports
{
    public class IncomeReportRowDTO
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
}

