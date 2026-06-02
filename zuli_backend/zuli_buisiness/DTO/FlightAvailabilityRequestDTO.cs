using System;
using System.Collections.Generic;

namespace zuli_Business.DTO
{
    public class FlightAvailabilityRequestDTO
    {
        public int Seats { get; set; }
        public List<FlightSegmentAvailabilityDTO> Segments { get; set; } = new List<FlightSegmentAvailabilityDTO>();
    }
}