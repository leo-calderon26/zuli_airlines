using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Buisiness.DTO.External
{
    public class RetrievedFlightDTO
    {
        public Guid flightGUID { get; set; }
        public DateTime departureTime { get; set; }
        public DateTime arrivalTime { get; set; }
        public int duration { get; set; }
        public string departureAirportCode { get; set; }
        public string departureAirportName { get; set; }
        public string departureAirportCity { get; set; }
        public string arrivalAirportCode { get; set; }
        public string arrivalAirportName { get; set; }
        public string arrivalAirportCity { get; set; }
        public decimal touristPrice { get; set; }
        public decimal firstClassPrice { get; set; }
        public decimal carryOnPrice { get; set; }
        public decimal checkedPrice { get; set; }
    }
}
