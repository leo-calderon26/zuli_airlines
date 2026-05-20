using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace zuli_Buisiness.DTO
{
    public class AuthorizationDTO
    {
        [Required(ErrorMessage = "El nombre de la aerolínea es requerido")]
        public string airlineName { get; set; }
    }
}
