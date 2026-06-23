using System.ComponentModel.DataAnnotations;
namespace zuli_Business.DTO.Reports
{
    public class IncomeReportRequestDTO
    {
        [Required(ErrorMessage = "Es obligatorio ingresar un año")]
        [Range(2026, int.MaxValue, ErrorMessage = "El año debe ser 2026 o posterior")]
        public int Year { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public int? AirlineId { get; set; }
    }    
}
