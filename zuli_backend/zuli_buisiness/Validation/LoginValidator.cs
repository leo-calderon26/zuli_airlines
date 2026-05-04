using System.Text.RegularExpressions;
using zuli_Business.DTO;
using zuli_Data.Exceptions;

namespace zuli_Business.Validation
{
    public class LoginValidator
    {
        public void Validate(LoginRequestDTO? request)
        {
            Dictionary<string, string[]> errors = [];

            if (request == null)
            {
                errors["request"] = ["Solicitud inválida."];
                throw new ZuliValidationException(errors);
            }

            if (string.IsNullOrWhiteSpace(request.BusinessEmail))
            {
                errors["businessEmail"] = ["El correo es obligatorio."];
            }
            else if (!Regex.IsMatch(request.BusinessEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errors["businessEmail"] = ["El correo no tiene un formato válido."];
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errors["password"] = ["La contraseña es obligatoria."];
            }

            if (errors.Count > 0)
            {
                throw new ZuliValidationException(errors);
            }
        }
    }
}