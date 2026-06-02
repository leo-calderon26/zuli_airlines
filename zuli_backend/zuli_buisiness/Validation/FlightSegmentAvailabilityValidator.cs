using FluentValidation;
using zuli_Business.DTO;

namespace zuli_Business.Validation
{
    public class FlightSegmentAvailabilityValidator : AbstractValidator<FlightSegmentAvailabilityDTO>
    {
        public FlightSegmentAvailabilityValidator()
        {
            RuleFor(x => x.FlightRouteId)
                .GreaterThan(0).WithMessage("Debe especificar una ruta de vuelo válida.");
            
            RuleFor(x => x.DepartureDate)
                .NotEmpty().WithMessage("Debe especificar la fecha de salida.");
        }
    }
}