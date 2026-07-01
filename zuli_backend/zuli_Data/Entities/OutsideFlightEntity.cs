using System;

namespace zuli_Data.Entities
{
    public class OutsideFlightEntity
    {
        public int AirlineId { get; set; }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
        public TimeSpan RealDepartureTime { get; set; }
        public TimeSpan RealArrivalTime { get; set; }
        public int DurationOnSeconds { get; set; }
        public decimal? CarryOnPrice { get; set; }
        public decimal? CheckedPrice { get; set; }
        public string ArrivalAirportCode { get; set; }
        public string ArrivalAirportName { get; set; }
        public string ArrivalAirportCity { get; set; }
        public string DepartureAirportCode { get; set; }
        public string DepartureAirportName { get; set; }
        public string DepartureAirportCity { get; set; }
        public int Frequency { get; set; }
    }
}
