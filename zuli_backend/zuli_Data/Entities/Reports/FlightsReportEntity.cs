using System;

namespace zuli_Data.Entities.Reports
{
    public class FlightsReportEntity
    {
        public DateTime Date { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public int FlightNumber { get; set; }
        public int FirstClassPassengers { get; set; }
        public int EconomyClassPassengers { get; set; }
        public string Airline { get; set; } = string.Empty;
        public decimal PassengerSales { get; set; }
        public decimal BaggageSales { get; set; }
        public decimal TotalSales { get; set; }
    }
}