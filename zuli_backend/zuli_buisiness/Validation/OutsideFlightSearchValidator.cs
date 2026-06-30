using FluentValidation;
using zuli_Business.DTO;

namespace zuli_Business.Validation
{
    public class OutsideFlightSearchValidator : AbstractValidator<OutsideFlightRequestDTO>
    {
        public OutsideFlightSearchValidator()
        {
            RuleFor(x => x.EarliestDeparture)
                .GreaterThanOrEqualTo(_ => DateTime.Today)
                .WithMessage("La fecha de salida no puede ser anterior a hoy.");
        }
    }
}