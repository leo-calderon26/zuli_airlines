using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

using System;
using System.ComponentModel.DataAnnotations;

namespace zuli_Business.DTO
{
    public class OutsideFlightRequestDTO
    {

        [Required(ErrorMessage = "El destino es requerido")]
        [StringLength(3, ErrorMessage = "El código de destino debe ser de 3 caracteres")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "La fecha más pronta de salida es requerida")]
        [DataType(DataType.Date)]
        public DateTime EarliestDeparture { get; set; }

        [Required(ErrorMessage = "La fecha más tarde de salida es requerida")]
        [DataType(DataType.Date)]
        public DateTime LatestDeparture { get; set; }

        [Range(1, 1000, ErrorMessage = "Debe indicar la cantidad de pasajeros")]
        public int QuantityOfPassengers { get; set; }
    }
}