using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.ComponentModel.DataAnnotations;

namespace zuli_Business.DTO
{
    public class FlightRouteDTO
    {
        [Required(ErrorMessage = "La frecuencia del vuelo es requerida")]
        public int frequency { get; set; }
        [DataType(DataType.Date)]
        public DateTime scheduledArrivalTime { get; set; }
        [DataType(DataType.Date)]
        public DateTime scheduledDepartureTime { get; set; }
        // Este sera calculado por la persona que ingrese las rutas y es en segundos
        [Required(ErrorMessage = "Es necesario ingresar la duración estimada")]
        public int estimatedDuration { get; set; }
        [Required(ErrorMessage = "Es necesario el Id del admin")]
        public Guid adminId { get; set; }
        [Required(ErrorMessage = "Es necesario el Id de la aerolinea")]
        public int airlineId { get; set; }
        [Required(ErrorMessage = "Es necesario el aeropuerto de llegada")]
        public string arrivalAirport {  get; set; }
        [Required(ErrorMessage = "Es necesario el aeropuerto de salida")]
        public string departureAirport { get; set; }
    }
}
