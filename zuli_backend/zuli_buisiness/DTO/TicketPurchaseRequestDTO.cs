using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace zuli_Business.DTO
{
    public class TicketPurchaseRequestDTO
    {
        public Guid? FlightId { get; set; }

        [Required(ErrorMessage = "La clase de vuelo es requerida")]
        [StringLength(20, ErrorMessage = "La clase no puede exceder 20 caracteres")]
        public string FlightClass { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe haber al menos un pasajero")]
        [MinLength(1, ErrorMessage = "Debe haber al menos un pasajero")]
        public List<PassengerTicketDTO> Passengers { get; set; } = new();

        [Required(ErrorMessage = "Los datos del comprador son requeridos")]
        public BuyerTicketDTO Buyer { get; set; } = new();

        [Required(ErrorMessage = "El método de pago es requerido")]
        [StringLength(30, ErrorMessage = "El método de pago no puede exceder 30 caracteres")]
        public string PaymentMethod { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El origen no puede exceder 20 caracteres")]
        public string ReservationOrigin { get; set; } = "Web";

        public Guid? ReturnFlightId { get; set; }
    }

    public class BuyerTicketDTO
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es requerido")]
        public string FirstLastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El segundo apellido es requerido")]
        public string SecondLastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "La fecha de nacimiento debe tener formato YYYY-MM-DD")]
        public string BirthDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string Phone { get; set; } = string.Empty;
    }
}