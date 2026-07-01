using System;

namespace zuli_Business.DTO.Reports
{
    public class FlightsReportDTO
    {
        public DateTime Date { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public int? FlightNumber { get; set; }
        public int FirstClassPassengers { get; set; }
        public int EconomyClassPassengers { get; set; }
        public string? Airline { get; set; }
        public decimal PassengerSales { get; set; }
        public decimal BaggageSales { get; set; }
        public decimal TotalSales { get; set; }
    }
}