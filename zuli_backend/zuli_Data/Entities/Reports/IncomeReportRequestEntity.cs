namespace zuli_Data.Entities.Reports
{
    public class  IncomeReportRequestEntity
    {
        public int Year { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public int? AirlineId { get; set; }
    }    
}