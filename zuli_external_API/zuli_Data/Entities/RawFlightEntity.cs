using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Data.Entities
{
    public class RawFlightEntity
    {
        public Guid FlightGUID { get; set; }
        public int FlightRouteId { get; set; }
        public int EstimatedDuration { get; set; }
        public string DepartureAiportCode { get; set; }
        public string DepartureAiportName { get; set; }
        public string DepartureCityName { get; set; }
        public string ArrivalAiportCode { get; set; }
        public string ArrivalAirportName { get; set; }
        public string ArrivalAirportCity { get; set; }
        public int Frequency { get; set;  }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
        public decimal CarryOnPrice { get; set; }
        public decimal CheckedPrice { get; set; }
        public TimeSpan ScheduledDepartureTime { get; set; }
        public TimeSpan ScheduledArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}