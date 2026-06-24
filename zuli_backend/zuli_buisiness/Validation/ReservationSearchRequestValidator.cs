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
                .Length(ReservationSearchRequestDTO.RESERVATION_CODE_LENGTH)
                .WithMessage($"El código debe tener exactamente {ReservationSearchRequestDTO.RESERVATION_CODE_LENGTH} caracteres.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido es requerido.");
        }
    }
}