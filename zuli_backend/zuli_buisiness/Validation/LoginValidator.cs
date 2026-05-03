using System.Text.RegularExpressions;
using zuli_Business.DTO;

namespace zuli_Business.Validation
{
    public class LoginValidator
    {
        public string? Validate(LoginRequestDTO? request)
        {
            if (request == null)
            {
                return "Solicitud inválida.";
            }

            if (string.IsNullOrWhiteSpace(request.BusinessEmail))
            {
                return "El correo es obligatorio.";
            }

            if (!Regex.IsMatch(request.BusinessEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return "El correo no tiene un formato válido.";
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return "La contraseña es obligatoria.";
            }

            return null;
        }
    }
}