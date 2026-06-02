using FluentValidation;

namespace zuli_Business.Validation
{
    public class AirportSearchValidator : AbstractValidator<string>
    {
        public AirportSearchValidator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("La búsqueda no puede venir vacía o con espacios en blanco.");

            RuleFor(x => x)
                .MaximumLength(100).WithMessage("La búsqueda no puede superar los 100 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x));
        }
    }
}