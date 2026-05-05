using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Buisiness.DTO
{
    public class FlightSearchResponseDTO
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string DepartureTimeText { get; set; }
        public string ArrivalTimeText { get; set; }
        public string ArrivalDateText { get; set; }
        public string TotalDurationText { get; set; }
        public int Stops { get; set; }
        public string LayoverAirports { get; set; }
        public decimal TotalTouristPrice { get; set; }
        public decimal TotalFirstClassPrice { get; set; }
    }

    public class PagedFlightResponseDTO
    {
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<FlightSearchResponseDTO> Flights { get; set; }
    }
}