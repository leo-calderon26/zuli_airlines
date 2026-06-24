using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Business.DTO
{
    public class AuthorizationDTO
    {
        [Required(ErrorMessage = "El nombre de la aerolínea es requerido")]
        public string airlineName { get; set; }
        [Required(ErrorMessage = "La api key es requerida")]
        public string apiKey { get; set; }
    }
}
