using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

using System;
using System.ComponentModel.DataAnnotations;

namespace zuli_Business.DTO
{
    public class FlightSearchRequestDTO
    {
        [Required(ErrorMessage = "El origen es requerido")]
        [StringLength(3, ErrorMessage = "El código de origen debe ser de 3 caracteres")]
        public string Origin { get; set; }

        [Required(ErrorMessage = "El destino es requerido")]
        [StringLength(3, ErrorMessage = "El código de destino debe ser de 3 caracteres")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "La fecha de salida es requerida")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Range(1, 1000, ErrorMessage = "Debe buscar al menos 1 asiento")]
        public int Seats { get; set; }

        public bool IsRoundTrip { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool DirectFlightsOnly { get; set; } 
        public string FlightClass { get; set; } = "Turista";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}