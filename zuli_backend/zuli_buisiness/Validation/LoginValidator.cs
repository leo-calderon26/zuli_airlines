using System.Text.RegularExpressions;
using zuli_Business.DTO;
using zuli_Data.Exceptions;

namespace zuli_Business.Validation
{
    public class LoginValidator
    {
        public void Validate(LoginRequestDTO? request)
        {
            var errors = new Dictionary<string, List<string>>();

            if (request == null)
            {
                errors["request"] = new List<string>
                {
                    "Solicitud inválida."
                };

                throw new ZuliValidationException(errors);
            }

            if (string.IsNullOrWhiteSpace(request.BusinessEmail))
            {
                errors["businessEmail"] = new List<string>
                {
                    "El correo es obligatorio."
                };
            }
            else if (!Regex.IsMatch(request.BusinessEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errors["businessEmail"] = new List<string>
                {
                    "El correo no tiene un formato válido."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errors["password"] = new List<string>
                {
                    "La contraseña es obligatoria."
                };
            }

            if (errors.Count > 0)
            {
                throw new ZuliValidationException(errors);
            }
        }
    }
}