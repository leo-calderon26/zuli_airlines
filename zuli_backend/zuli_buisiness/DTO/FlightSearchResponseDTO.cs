using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Business.DTO
{
    public class FlightSearchResponseDTO
    {
        public Guid? FlightId { get; set; }
        public string PathIds { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string DepartureTimeText { get; set; }
        public string ArrivalTimeText { get; set; }
        public string ArrivalDateText { get; set; }
        public string TotalDurationText { get; set; }
        public int Stops { get; set; }
        public decimal TotalTouristPrice { get; set; }
        public decimal TotalFirstClassPrice { get; set; }
        public decimal? CarryOnPrice { get; set; }
        public decimal? CheckedPrice { get; set; }
        public decimal MaxWeightPerBag { get; set; }
        public decimal CheckedBagMultiplier { get; set; }
        public List<FlightSegmentDTO> Segments { get; set; } = new List<FlightSegmentDTO>();
    }
}