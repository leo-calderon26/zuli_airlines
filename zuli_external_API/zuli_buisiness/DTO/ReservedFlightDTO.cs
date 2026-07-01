using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Business.DTO
{
    public class ReservedFlightDTO
    {
        public DateTime RealDepartureTime { get; set; }
        public DateTime RealArrivalTime { get; set; }
        public int Duration { get; set; }
        public string ArrivalAiportCode { get; set; }
        public string ArrivalAirportName { get; set; }
        public string ArrivalAirportCity { get; set; }
        public string DepartureAirportCode { get; set; }
        public string DepartureAirportName { get; set; }
        public string DepartureCityName { get; set; }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
        public decimal CarryOnPrice { get; set; }
        public decimal CheckedPrice { get; set; }
        
    }
}