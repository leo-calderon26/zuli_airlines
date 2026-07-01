using System;

namespace zuli_Data.Entities.Filters
{
    public class FlightsReportFilterEntity
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public string? FlightClass { get; set; }
    }
}