using System.ComponentModel.DataAnnotations;

namespace zuli_Business.DTO
{
    public class FlightRouteDTO
    {
        [Required(ErrorMessage = "La frecuencia del vuelo es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La frecuencia debe ser mayor que cero")]
        public int frequency { get; set; }
        [DataType(DataType.Date)]
        public DateTime scheduledArrivalTime { get; set; }
        [DataType(DataType.Date)]
        public DateTime scheduledDepartureTime { get; set; }
        // Este sera calculado por la persona que ingrese las rutas y es en segundos
        [Required(ErrorMessage = "Es necesario ingresar la duración estimada")]
        public int estimatedDuration { get; set; }
        [Required(ErrorMessage = "Es necesario el Id de negocios")]
        public string businessId { get; set; }
        [Required(ErrorMessage = "Es necesario el Id de la aerolinea")]
        [Range(1, int.MaxValue, ErrorMessage = "El airlineId debe ser un numero entero positivo")]
        public int airlineId { get; set; }
        [Required(ErrorMessage = "Es necesario el aeropuerto de llegada")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El aeropuerto de llegada debe tener 3 caracteres")]
        public string arrivalAirport {  get; set; }
        [Required(ErrorMessage = "Es necesario el aeropuerto de salida")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El aeropuerto de salida debe tener 3 caracteres")]
        public string departureAirport { get; set; }
    }
}
