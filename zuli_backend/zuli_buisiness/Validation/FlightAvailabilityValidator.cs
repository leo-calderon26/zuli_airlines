using FluentValidation;
using zuli_Business.DTO;

namespace zuli_Business.Validation
{
    public class FlightAvailabilityValidator : AbstractValidator<FlightAvailabilityRequestDTO>
    {
        public FlightAvailabilityValidator()
        {
            RuleFor(x => x.Seats)
                .GreaterThan(0).WithMessage("Debe buscar al menos 1 asiento.");
            
            RuleFor(x => x.Segments)
                .NotEmpty().WithMessage("Debe enviar al menos un trayecto.");
            
            RuleForEach(x => x.Segments).SetValidator(new FlightSegmentAvailabilityValidator());
        }
    }
}