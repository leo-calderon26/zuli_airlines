using System;

namespace zuli_Business.DTO.Filters
{
    public class FlightsReportFilterDTO
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public string? FlightClass { get; set; }
    }
}