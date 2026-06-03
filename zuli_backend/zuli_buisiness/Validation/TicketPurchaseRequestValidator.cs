using FluentValidation;
using zuli_Business.DTO;

namespace zuli_Business.Validation
{
    public class TicketPurchaseRequestValidator : AbstractValidator<TicketPurchaseRequestDTO>
    {
        public TicketPurchaseRequestValidator()
        {
            RuleFor(request => request.Passengers)
                .NotEmpty()
                .WithName("passengers")
                .WithMessage("Debe haber al menos un pasajero.");

            RuleForEach(request => request.Passengers)
                .ChildRules(passenger =>
                {
                    passenger.RuleFor(p => p.CheckedBaggage)
                        .InclusiveBetween(0, 5)
                        .WithName("checkedBaggage")
                        .WithMessage("Cada pasajero puede llevar máximo 5 maletas documentadas.");

                    passenger.RuleFor(p => p.CarryOn)
                        .InclusiveBetween(0, 1)
                        .WithName("carryOn")
                        .WithMessage("Cada pasajero puede llevar máximo un equipaje de mano.");
                });
        }
    }
}