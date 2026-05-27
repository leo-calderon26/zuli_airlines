using System.ComponentModel.DataAnnotations;

namespace zuli_Business.DTO
{
    public class BaggageItemDTO
    {
        [Range(0, 100, ErrorMessage = "El peso debe estar entre 0 y 100 kg")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "El tamaño del equipaje es requerido")]
        [StringLength(20, ErrorMessage = "El tamaño no puede exceder 20 caracteres")]
        public string Size { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de equipaje es requerido")]
        [StringLength(30, ErrorMessage = "El tipo no puede exceder 30 caracteres")]
        public string? Type { get; set; }
    }
}
