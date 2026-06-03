using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Data.Entities
{
    public class FlightRouteEntity
    {
        public int flightRouteId { get; set; }
        public int frequency { get; set; }
        public TimeSpan scheduledArrivalTime { get; set; }
        public TimeSpan scheduledDepartureTime { get; set; }
        // Este sera calculado por la persona que ingrese las rutas y es en segundos
        public int estimatedDuration { get; set; }
        public Guid adminId { get; set; }
        public int airlineId { get; set; }
        public string arrivalAirport {  get; set; }
        public string departureAirport { get; set; }
        public string aircraftId { get; set; }

        public decimal carryOnPrice { get; set; }

        public decimal checkedPrice { get; set; }

        public decimal touristPrice { get; set; }

        public decimal firstClassPrice { get; set; }
        public decimal multiplier { get; set; }
    }
}
