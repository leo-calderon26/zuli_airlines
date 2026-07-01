using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Data.Entities
{
    public class RawFlightEntity
    {
        public int FlightRouteId { get; set; }
        public Guid? FlightId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int EstimatedDuration { get; set; }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
        public decimal? CarryOnPrice { get; set; }
        public decimal? CheckedPrice { get; set; }
        public decimal MaxWeightPerBag { get; set; }
        public decimal CheckedBagMultiplier { get; set; }
        public string AirlineName { get; set; }
    }
}