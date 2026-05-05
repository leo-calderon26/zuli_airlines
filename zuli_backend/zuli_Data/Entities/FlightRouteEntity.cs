using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Data.Entities
{
    public class FlightRouteEntity
    {
        public int frequency { get; set; }
        public DateTime scheduledArrivalTime { get; set; }
        public DateTime scheduledDepartureTime { get; set; }
        // Este sera calculado por la persona que ingrese las rutas y es en segundos
        public int estimatedDuration { get; set; }
        public Guid adminId { get; set; }
        public int airlineId { get; set; }
        public string arrivalAirport {  get; set; }
        public string departureAirport { get; set; }
    }
}
