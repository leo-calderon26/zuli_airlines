using System.Text.RegularExpressions;
using zuli_Business.DTO;
using zuli_Data.Exceptions;

namespace zuli_Business.Validation
{
    public class RegisterUserValidator
    {
        public void Validate(RegisterUserRequestDTO? request)
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

            ValidateNationalId(request.NationalId, errors);
            ValidateEmail(request.BusinessEmail, errors);
            ValidateTextField("firstName", request.FirstName, "El primer nombre", errors);
            ValidateTextField("firstLastName", request.FirstLastName, "El primer apellido", errors);
            ValidateTextField("secondLastName", request.SecondLastName, "El segundo apellido", errors);
            ValidateUserRole(request.UserRole, errors);

            if (errors.Count > 0)
            {
                throw new ZuliValidationException(errors);
            }
        }

        private static void ValidateNationalId(string nationalId, Dictionary<string, List<string>> errors)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
            {
                errors["nationalId"] = new List<string>
                {
                    "La cédula es obligatoria."
                };

                return;
            }

            if (!Regex.IsMatch(nationalId.Trim(), @"^\d{9}$"))
            {
                errors["nationalId"] = new List<string>
                {
                    "La cédula debe tener exactamente 9 dígitos, sin espacios ni guiones."
                };
            }
        }

        private static void ValidateEmail(string email, Dictionary<string, List<string>> errors)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                errors["businessEmail"] = new List<string>
                {
                    "El correo institucional es obligatorio."
                };

                return;
            }

            if (!Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errors["businessEmail"] = new List<string>
                {
                    "El correo institucional no tiene un formato válido."
                };
            }
        }

        private static void ValidateTextField(
            string fieldName,
            string value,
            string displayName,
            Dictionary<string, List<string>> errors)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors[fieldName] = new List<string>
                {
                    $"{displayName} es obligatorio."
                };

                return;
            }

            if (value.Trim().Length > 50)
            {
                errors[fieldName] = new List<string>
                {
                    $"{displayName} no puede superar los 50 caracteres."
                };

                return;
            }

            if (!Regex.IsMatch(value.Trim(), @"^[\p{L} '-]+$"))
            {
                errors[fieldName] = new List<string>
                {
                    $"{displayName} solo puede contener letras, espacios, apóstrofes o guiones."
                };
            }
        }

        private static void ValidateUserRole(string userRole, Dictionary<string, List<string>> errors)
        {
            if (string.IsNullOrWhiteSpace(userRole))
            {
                errors["userRole"] = new List<string>
                {
                    "El tipo de usuario es obligatorio."
                };

                return;
            }

            string trimmedRole = userRole.Trim();

            if (trimmedRole != "Administrator" && trimmedRole != "Operator")
            {
                errors["userRole"] = new List<string>
                {
                    "El tipo de usuario no es válido."
                };
            }
        }
    }
}