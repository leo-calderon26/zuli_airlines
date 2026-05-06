using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Data.Entities
{
    public class FlightSearchEntity
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int TotalDurationSeconds { get; set; }
        public decimal TotalTouristPrice { get; set; }
        public decimal TotalFirstClassPrice { get; set; }
        public int Stops { get; set; }
        public string LayoverAirports { get; set; } 
        public int TotalRecordsDb { get; set; }
    }
}