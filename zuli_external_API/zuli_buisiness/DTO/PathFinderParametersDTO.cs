using System;
using System.Collections.Generic;
using zuli_Data.Entities;

namespace zuli_Business.DTO
{
    public class PathFinderParametersDTO
    {
        public IEnumerable<RawFlightEntity> FlightPool { get; set; } = new List<RawFlightEntity>();
        public string Destination { get; set; } = string.Empty;
        public DateTime EarliestDeparture { get; set; }
        public DateTime LatestDeparture { get; set; }
    }
}