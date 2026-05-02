using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Business.DTO
{
    public class FlightDTO
    {
        public int flightId { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public TimeOnly realArrivalTime { get; set; }
        public TimeOnly checkInStartTime { get; set; }
        public TimeOnly checkInDeadline { get; set; }
        public TimeOnly realDepartureTime { get; set; }
    }
}
