using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace zuli_Business.DTO
{
    public class TicketConfirmationDTO
    {
        [Required]
        [StringLength(8, ErrorMessage = "El código de confirmación debe tener 8 caracteres")]
        public string ConfirmationCode { get; set; } = string.Empty;

        public int ReservationId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El total debe ser un valor positivo")]
        public decimal TotalPayment { get; set; }

        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; }

        [StringLength(3, ErrorMessage = "El código de origen debe tener 3 caracteres")]
        public string FlightOrigin { get; set; } = string.Empty;

        [StringLength(3, ErrorMessage = "El código de destino debe tener 3 caracteres")]
        public string FlightDestination { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "La clase no puede exceder 20 caracteres")]
        public string FlightClass { get; set; } = string.Empty;

        public List<PassengerSummaryDTO> Passengers { get; set; } = new();
    }

    public class PassengerSummaryDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string FirstLastName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El segundo apellido no puede exceder 100 caracteres")]
        public string SecondLastName { get; set; } = string.Empty;
    }
}
