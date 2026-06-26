using FluentValidation;
using zuli_Business.DTO;

namespace zuli_Business.Validation
{
    public class OutsideFlightSearchValidator : AbstractValidator<OutsideFlightRequestDTO>
    {
        public OutsideFlightSearchValidator()
        {
            RuleFor(x => x.Destination)
                .NotEmpty().Length(3)
                .WithMessage("El destino debe tener exactamente 3 caracteres.");
            RuleFor(x => x.QuantityOfPassengers).GreaterThan(0)
                .WithMessage("Debe buscar al menos 1 asiento.");
            RuleFor(x => x.EarliestDeparture)
                .GreaterThanOrEqualTo(_ => DateTime.Today)
                .WithMessage("La fecha de salida no puede ser anterior a hoy.");
        }
    }
}