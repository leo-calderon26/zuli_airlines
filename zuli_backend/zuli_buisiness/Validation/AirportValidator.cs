using System.Linq;
using FluentValidation;
using zuli_Business.DTO;

namespace zuli_Business.Validation
{
    public static class AirportAtributes
    {
        public const string CODE = "AirportCode";
        public const string NAME = "Name";
        public const string COUNTRY = "Country";
        public const string CITY = "City";
        public const string ADMINID = "AdminId";
    }

    public class AirportValidator : AbstractValidator<AirportDTO>
    {
        public AirportValidator()
        {
            RuleFor(x => x.airportCode)
                .NotEmpty().WithMessage("El codigo del aeropuerto no puede venir vacio o con espacios en blanco")
                .OverridePropertyName(AirportAtributes.CODE);

            RuleFor(x => x.airportCode)
                .Length(3)
                .When(x => !string.IsNullOrWhiteSpace(x.airportCode))
                .WithMessage("El codigo del aeropuerto debe tener exactamente 3 caracteres")
                .OverridePropertyName(AirportAtributes.CODE);

            RuleFor(x => x.airportCode)
                .Must(code => !code.Contains(' '))
                .When(x => !string.IsNullOrWhiteSpace(x.airportCode))
                .WithMessage("El codigo del aeropuerto no puede contener espacios")
                .OverridePropertyName(AirportAtributes.CODE);

            RuleFor(x => x.airportCode)
                .Must(MiscValidor.ContainsChars)
                .When(x => !string.IsNullOrWhiteSpace(x.airportCode))
                .WithMessage("El codigo del aeropuerto solo puede contener letras")
                .OverridePropertyName(AirportAtributes.CODE);

            RuleFor(x => x.name)
                .NotEmpty().WithMessage("El nombre del aeropuerto no puede venir vacio o con espacios en blanco")
                .MaximumLength(100).WithMessage("El nombre del aeropuerto no puede medir más de 100 caracteres")
                .OverridePropertyName(AirportAtributes.NAME);

            RuleFor(x => x.country)
                .NotEmpty().WithMessage("El pais no puede venir vacio o con espacios en blanco")
                .MaximumLength(60).WithMessage("El pais no puede medir más de 60 caracteres")
                .OverridePropertyName(AirportAtributes.COUNTRY);

            RuleFor(x => x.city)
                .NotEmpty().WithMessage("La ciudad no puede venir vacia o con espacios en blanco")
                .MaximumLength(60).WithMessage("La ciudad no puede medir más de 60 caracteres")
                .OverridePropertyName(AirportAtributes.CITY);

            RuleFor(x => x.businessId)
                .NotEmpty().WithMessage("El adminId no puede venir vacio")
                .OverridePropertyName(AirportAtributes.ADMINID);
        }
    }
}