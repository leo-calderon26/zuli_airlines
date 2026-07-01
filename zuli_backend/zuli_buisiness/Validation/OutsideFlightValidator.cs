using FluentValidation;
using zuli_Business.DTO;

namespace zuli_Business.Validation
{
    public class OutsideFlightValidator : AbstractValidator<OutsideFlightDTO>
    {
        public OutsideFlightValidator()
        {
            RuleFor(x => x.TouristPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El precio de clase turista no puede ser negativo.");

            RuleFor(x => x.FirstClassPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El precio de primera clase no puede ser negativo.");

            RuleFor(x => x.Duration)
                .NotEmpty()
                .WithMessage("La duración es obligatoria.");

            RuleFor(x => x.CarryOnPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.CarryOnPrice.HasValue)
                .WithMessage("El precio del equipaje de mano no puede ser negativo.");

            RuleFor(x => x.CheckedPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.CheckedPrice.HasValue)
                .WithMessage("El precio del equipaje documentado no puede ser negativo.");
        }
    }
}