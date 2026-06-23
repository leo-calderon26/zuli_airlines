using FluentValidation;
using zuli_Business.DTO.ReservationSearch;

namespace zuli_Business.Validation
{
    public class ReservationSearchRequestValidator : AbstractValidator<ReservationSearchRequestDTO>
    {
        public ReservationSearchRequestValidator()
        {
            RuleFor(x => x.ReservationCode)
                .NotEmpty().WithMessage("El código de reserva es requerido.")
                .Length(ReservationSearchRequestDTO.ReservationCodeLength)
                .WithMessage($"El código debe tener exactamente {ReservationSearchRequestDTO.ReservationCodeLength} caracteres.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido es requerido.");
        }
    }
}