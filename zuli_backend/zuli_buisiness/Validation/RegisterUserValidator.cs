using FluentValidation;
using zuli_Business.DTO;
using System.Text.RegularExpressions;

namespace zuli_Business.Validation
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserRequestDTO>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("La cédula es obligatoria.")
                .Must(id => Regex.IsMatch(id.Trim(), @"^\d{9}$"))
                .When(x => !string.IsNullOrWhiteSpace(x.NationalId))
                .WithMessage("La cédula debe tener exactamente 9 dígitos, sin espacios ni guiones.");

            RuleFor(x => x.BusinessEmail)
                .NotEmpty().WithMessage("El correo institucional es obligatorio.")
                .Must(email => Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                .When(x => !string.IsNullOrWhiteSpace(x.BusinessEmail))
                .WithMessage("El correo institucional no tiene un formato válido.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El primer nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El primer nombre no puede superar los 50 caracteres.")
                .Matches(@"^[\p{L} '-]+$").When(x => !string.IsNullOrWhiteSpace(x.FirstName))
                .WithMessage("El primer nombre solo puede contener letras, espacios, apóstrofes o guiones.");

            RuleFor(x => x.FirstLastName)
                .NotEmpty().WithMessage("El primer apellido es obligatorio.")
                .MaximumLength(50).WithMessage("El primer apellido no puede superar los 50 caracteres.")
                .Matches(@"^[\p{L} '-]+$").When(x => !string.IsNullOrWhiteSpace(x.FirstLastName))
                .WithMessage("El primer apellido solo puede contener letras, espacios, apóstrofes o guiones.");

            RuleFor(x => x.SecondLastName)
                .NotEmpty().WithMessage("El segundo apellido es obligatorio.")
                .MaximumLength(50).WithMessage("El segundo apellido no puede superar los 50 caracteres.")
                .Matches(@"^[\p{L} '-]+$").When(x => !string.IsNullOrWhiteSpace(x.SecondLastName))
                .WithMessage("El segundo apellido solo puede contener letras, espacios, apóstrofes o guiones.");

            RuleFor(x => x.UserRole)
                .NotEmpty().WithMessage("El tipo de usuario es obligatorio.")
                .Must(role => role.Trim() == "Administrator" || role.Trim() == "Operator")
                .When(x => !string.IsNullOrWhiteSpace(x.UserRole))
                .WithMessage("El tipo de usuario no es válido.");
        }
    }
}