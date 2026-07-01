using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Business.DTO
{
    public class PassengerInfoDTO
    {
        public bool carryOn { get; set; } = false;
        [Range(0, 5, ErrorMessage = "La cantidad de maletas no puede ser menor a cero ni mayor a 5")]
        public int Checked { get; set; } = 0;
        [Required(ErrorMessage = "El pasaporte del pasajero es requerido")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "El pasaporte debe ser de exactamente 9 caracteres")]
        public string Passport { get; set; } = string.Empty;
        [Required(ErrorMessage = "La fecha de vencimiento del pasaporte es requerida")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "La fecha de vencimiento debe tener formato YYYY-MM-DD")]
        public string PassportExpirationDate { get; set; } = string.Empty;
        [Required(ErrorMessage = "El código del país al que pertenece el pasaporte es requerido")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El código del país al que pertenece el pasaporte debe ser de exactamente 3 caracteres")]
        public string PassportCountry { get; set; } = string.Empty;
        [Required(ErrorMessage = "El nombre del pasajero es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El segundo apellido no puede exceder 100 caracteres")]
        public string LastName2 { get; set; } = string.Empty;
        [Required(ErrorMessage = "El género es requerido")]
        [RegularExpression("^[FHO]$", ErrorMessage = "La categoría debe ser f, h u o.")]
        public string Gender { get; set; } = string.Empty;
        [Required(ErrorMessage = "La fecha de nacimiento del pasajero es requerida")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "La fecha de vencimiento debe tener formato YYYY-MM-DD")]
        public string BirthDate { get; set; } = string.Empty;
    }
    public class BuyerInfoDTO {
        [Required(ErrorMessage = "El código de nacionalidad del comprador es requerido")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El código de nacionalidad del comprador debe ser de exactamente 3 caracteres")]
        public string Nationality { get; set; } = string.Empty;
        [Required(ErrorMessage = "El nombre del pasajero es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El segundo apellido no puede exceder 100 caracteres")]
        public string LastName2 { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        [StringLength(255, ErrorMessage = "El correo electrónico no puede exceder los 255 caracteres")]
        public string Email { get; set; } = string.Empty;
    }
    public class PaymentInfoDTO {
        [Required(ErrorMessage = "El número de tarjeta es requerida")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "El número de tarjeta debe ser de exactamente 16 dígitos")]
        public string CardNumber { get; set; }
        [Required(ErrorMessage = "La fecha de expiración de la tarjeta es requerida")]
        [RegularExpression(@"^\d{4}-\d{2}$", ErrorMessage = "La fecha de vencimiento debe tener formato YYYY-MM")]
        public string CardExpiration { get; set; }
        [Required(ErrorMessage = "El código CVV de la tarjeta es requerido")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "El código CVV de la tarjeta debe ser de 3 o 4 caracteres")]
        public string CVV { get; set; }
        [Required(ErrorMessage = "El nombre del usuario de la tarjeta es requerido")]
        [StringLength(256, ErrorMessage = "El nombre del usuario de la tarjeta no puede exceder los 256 caracteres")]
        public string CardHolderName { get; set; }
    }
    public class ReservationRequestDTO
    {
        // [Required(ErrorMessage = "El api key es requerido")]
        // public string apiKey { get; set; } = string.Empty;
        [Required(ErrorMessage = "El código del vuelo es requerido")]
        public Guid flightGUID { get; set; }
        public bool firstClass { get; set; } = false;
        public List<PassengerInfoDTO> passengers { get; set; }
        public BuyerInfoDTO buyer { get; set; }
        public PaymentInfoDTO payment { get; set; }
    }
}
