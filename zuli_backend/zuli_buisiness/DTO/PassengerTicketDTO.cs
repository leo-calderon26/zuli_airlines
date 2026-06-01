using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace zuli_Business.DTO
{
    public class PassengerTicketDTO
    {
        [Required(ErrorMessage = "El nombre del pasajero es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string FirstLastName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El segundo apellido no puede exceder 100 caracteres")]
        public string SecondLastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "La fecha de nacimiento debe tener formato YYYY-MM-DD")]
        public string BirthDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "El género es requerido")]
        [StringLength(20, ErrorMessage = "El género no puede exceder 20 caracteres")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "El país del pasaporte es requerido")]
        [StringLength(100, ErrorMessage = "El país no puede exceder 100 caracteres")]
        public string PassportCountry { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de vencimiento del pasaporte es requerida")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "La fecha de vencimiento debe tener formato YYYY-MM-DD")]
        public string PassportDueDate { get; set; } = string.Empty;

        [Range(0, 10, ErrorMessage = "El equipaje documentado debe estar entre 0 y 10")]
        public int CheckedBaggage { get; set; }

        public List<BaggageItemDTO> BaggageItems { get; set; } = new();

        [Range(0, 2, ErrorMessage = "El equipaje de mano debe estar entre 0 y 2")]
        public int CarryOn { get; set; }
    }
}
