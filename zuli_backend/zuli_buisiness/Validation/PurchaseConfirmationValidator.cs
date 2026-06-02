using FluentValidation;
using zuli_Business.DTO;

namespace zuli_Business.Validation
{
    public class PurchaseConfirmationValidator : AbstractValidator<PurchaseConfirmationPageDTO>
    {
        public PurchaseConfirmationValidator()
        {
            RuleFor(confirmation => confirmation.ReservationCode)
                .NotEmpty()
                .WithName("reservationCode")
                .WithMessage("La reserva no tiene código de reserva.");

            RuleFor(confirmation => confirmation.BuyerEmail)
                .NotEmpty()
                .WithName("buyerEmail")
                .WithMessage("La reserva no tiene correo del comprador.");

            RuleFor(confirmation => confirmation.BuyerName)
                .NotEmpty()
                .WithName("buyerName")
                .WithMessage("La reserva no tiene nombre del comprador.");

            RuleFor(confirmation => confirmation.Passengers)
                .NotNull()
                .Must(passengers => passengers.Count > 0)
                .WithName("passengers")
                .WithMessage("La reserva no tiene pasajeros asociados.");

            RuleFor(confirmation => confirmation.Flights)
                .NotNull()
                .Must(flights => flights.Count > 0)
                .WithName("flights")
                .WithMessage("La reserva no tiene vuelos asociados.");
        }
    }
}