using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Buisiness.DTO
{
    public class RequestedFlightDTO
    {
        [Required(ErrorMessage = "El código del aeropuerto de origen es requerido")]
        [StringLength(3, ErrorMessage = "El código del aeropuerto de origen debe ser de 3 caracteres")]
        public string origin { get; set; }
        [Required(ErrorMessage = "El código del aeropuerto de destino es requerido")]
        [StringLength(3, ErrorMessage = "El código del aeropuerto de destino debe ser de 3 caracteres")]
        public string destination { get; set; }
        [Required(ErrorMessage = "La fecha más pronta de partida es requerida")]
        [DataType(DataType.Date)]
        public DateTime earliestDeparture { get; set; }
        [Required(ErrorMessage = "La fecha más tarde de partida es requerida")]
        [DataType(DataType.Date)]
        public DateTime latestDeparture { get; set; }
        [Required(ErrorMessage = "La cantidad de pasajeros es requerida")]
        [Range(1, 1000, ErrorMessage = "La cantidad de pasajeros no puede ser menor a uno ni mayor a 1000")]
        public int passengersQuantity { get; set; }
    }
}
