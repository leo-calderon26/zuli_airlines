using System.Text.RegularExpressions;
using zuli_Business.DTO;
using zuli_Data.Exceptions;

namespace zuli_Business.Validation
{
    public class ActivateAccountValidator
    {
        public void Validate(ActivateAccountRequestDTO? request)
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

            if (string.IsNullOrWhiteSpace(request.Token))
            {
                errors["token"] = new List<string>
                {
                    "El token de activación es obligatorio."
                };
            }

            ValidatePassword(request.Password, request.ConfirmPassword, errors);

            if (errors.Count > 0)
            {
                throw new ZuliValidationException(errors);
            }
        }

        private static void ValidatePassword(
            string password,
            string confirmPassword,
            Dictionary<string, List<string>> errors)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                errors["password"] = new List<string>
                {
                    "La contraseña es obligatoria."
                };

                return;
            }

            if (password != confirmPassword)
            {
                errors["confirmPassword"] = new List<string>
                {
                    "Las contraseñas no coinciden."
                };
            }

            if (password.Length < 12)
            {
                errors["password"] = new List<string>
                {
                    "La contraseña debe tener al menos 12 caracteres."
                };

                return;
            }

            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                errors["password"] = new List<string>
                {
                    "La contraseña debe incluir al menos una letra mayúscula."
                };

                return;
            }

            if (!Regex.IsMatch(password, "[a-z]"))
            {
                errors["password"] = new List<string>
                {
                    "La contraseña debe incluir al menos una letra minúscula."
                };

                return;
            }

            if (!Regex.IsMatch(password, "[0-9]"))
            {
                errors["password"] = new List<string>
                {
                    "La contraseña debe incluir al menos un número."
                };

                return;
            }

            if (!Regex.IsMatch(password, @"[\W_]"))
            {
                errors["password"] = new List<string>
                {
                    "La contraseña debe incluir al menos un carácter especial."
                };
            }
        }
    }
}