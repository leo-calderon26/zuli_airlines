using System.Text.RegularExpressions;
using zuli_buisiness.DTO;

namespace zuli_buisiness.Validation
{
    public class LoginValidator
    {
        public string? Validate(LoginRequestDto? request)
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