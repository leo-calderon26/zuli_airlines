using FluentValidation;
using zuli_Business.DTO;

namespace zuli_Business.Validation
{
    public class FlightSearchValidator : AbstractValidator<FlightSearchRequestDTO>
    {
        public FlightSearchValidator()
        {
            RuleFor(x => x.Origin)
                .NotEmpty().Length(3)
                .WithMessage("El origen debe tener exactamente 3 caracteres.");

            RuleFor(x => x.Destination)
                .NotEmpty().Length(3)
                .WithMessage("El destino debe tener exactamente 3 caracteres.")
                .NotEqual(x => x.Origin).When(x => x.Origin != null)
                .WithMessage("El origen y destino no pueden ser iguales.");

            RuleFor(x => x.MaxLayovers).GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad máxima de escalas no puede ser menor a 0.");
            RuleFor(x => x.Seats).GreaterThan(0)
                .WithMessage("Debe buscar al menos 1 asiento.");
            RuleFor(x => x.Page).GreaterThan(0)
                .WithMessage("Página debe ser mayor a 0.");
            RuleFor(x => x.PageSize).GreaterThan(0)
                .WithMessage("Tamaño de página debe ser mayor a 0.");
        }
    }
}