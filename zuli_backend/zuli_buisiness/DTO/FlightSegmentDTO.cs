using System;
using System.Collections.Generic;
using System.Text;

namespace zuli_Business.DTO
{
    public class FlightSegmentDTO
    {
        public int FlightId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string DepartureTimeText { get; set; }
        public string ArrivalTimeText { get; set; }
        public string DurationText { get; set; }
        public string LayoverTimeText { get; set; }
        public string DepartureDateText { get; set; } = string.Empty;
        public decimal? CheckedPrice { get; set; }
        public decimal? CarryOnPrice { get; set; }
        public decimal CheckedBagMultiplier { get; set; }
        public string AirlineName { get; set; }
    }
}
