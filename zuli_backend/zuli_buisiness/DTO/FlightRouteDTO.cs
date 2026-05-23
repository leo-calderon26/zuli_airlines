using System.ComponentModel.DataAnnotations;

namespace zuli_Business.DTO
{
    public class FlightRouteDTO
    {
        [Required(ErrorMessage = "La frecuencia del vuelo es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La frecuencia debe ser mayor que cero")]
        public int frequency { get; set; }
        [DataType(DataType.Date)]
        public TimeSpan scheduledArrivalTime { get; set; }
        [DataType(DataType.Date)]
        public TimeSpan scheduledDepartureTime { get; set; }
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
        [Required(ErrorMessage = "Es necesario descoger una aeronave")]
        public string aircraftId { get; set; }
        [Required(ErrorMessage = "Es necesario ingresar un Precio del equipaje de mano")]
        [Range(1, int.MaxValue, ErrorMessage = "El precio del equipaje de mano debe ser mayor que cero")]
        public decimal carryOnPrice { get; set; }
        [Required(ErrorMessage = "Es necesario ingresar el precio de las maletas facturadas")]
        [Range(1, int.MaxValue, ErrorMessage = "El precio de las maletas facturadas debe ser mayor que cero")]
        public decimal checkedPrice { get; set; }
        [Required(ErrorMessage = "Es necesario ingresar el precio de clase Economica")]
        [Range(1, int.MaxValue, ErrorMessage = "El precio de la clase Economica debe ser mayor que cero")]
        public decimal touristPrice {get; set; }
        [Required(ErrorMessage = "Es necesario ingresar el precio de la primera clase")]
        [Range(1, int.MaxValue, ErrorMessage = "El precio de la primera clase debe ser mayor que cero")]
        public decimal firstClassPrice { get; set; }

        [Required(ErrorMessage = "Es necesario el multiplicador de equipaje documentado")]
        [Range(0.01, 100, ErrorMessage = "El multiplicador debe estar entre 0.01 y 100")]
        public decimal checkedBagMultiplier { get; set; }

        [Required(ErrorMessage = "Es necesario el peso maximo por maleta documentada")]
        [Range(1, 100, ErrorMessage = "El peso maximo debe estar entre 1 y 100 kg")]
        public decimal maxWeightPerBag { get; set; }
    }
}
