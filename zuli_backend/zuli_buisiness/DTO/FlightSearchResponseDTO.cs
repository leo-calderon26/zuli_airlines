using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Business.DTO
{
    public class FlightSearchResponseDTO
    {
        public string PathIds { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string DepartureTimeText { get; set; }
        public string ArrivalTimeText { get; set; }
        public string ArrivalDateText { get; set; }
        public string TotalDurationText { get; set; }
        public int Stops { get; set; }
        public decimal TotalPrice { get; set; }
        public List<FlightSegmentDTO> Segments { get; set; } = new List<FlightSegmentDTO>();
    }
}