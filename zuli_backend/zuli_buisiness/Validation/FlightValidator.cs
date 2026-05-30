using FluentValidation;
using zuli_Business.DTO;

namespace zuli_Business.Validation
{
    public class FlightValidator : AbstractValidator<FlightDTO>
    {
        public FlightValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Debe especificar el estado del vuelo")
                .MaximumLength(20).WithMessage("El estado no puede superar 20 caracteres");

            RuleFor(x => x.FlightDate).NotEmpty()
                .WithMessage("Debe especificar la fecha del vuelo");
            RuleFor(x => x.AircraftId).NotEmpty()
                .WithMessage("Debe especificar una aeronave valida");

            RuleFor(x => x.RealArrivalAirport)
                .Length(3).When(x => !string.IsNullOrEmpty(x.RealArrivalAirport))
                .WithMessage("El aeropuerto real de llegada debe tener 3 caracteres");

            RuleFor(x => x.RealDepartureAirport)
                .Length(3).When(x => !string.IsNullOrEmpty(x.RealDepartureAirport))
                .WithMessage("El aeropuerto real de salida debe tener 3 caracteres");

            RuleFor(x => x.BusinessId).NotEmpty()
                .WithMessage("Debe especificar un administrador valido");
            RuleFor(x => x.FlightRouteId).GreaterThan(0)
                .WithMessage("Debe especificar una ruta de vuelo valida");
            RuleFor(x => x.Duration).GreaterThan(0)
                .WithMessage("La duracion debe ser mayor a cero");

            RuleFor(x => x.FirstClassPrice).GreaterThanOrEqualTo(0)
                .WithMessage("Los precios no pueden ser negativos");
            RuleFor(x => x.TouristPrice).GreaterThanOrEqualTo(0)
                .WithMessage("Los precios no pueden ser negativos");
            RuleFor(x => x.CarryOnPrice).GreaterThanOrEqualTo(0)
                .When(x => x.CarryOnPrice.HasValue).WithMessage("Los precios no pueden ser negativos");
            RuleFor(x => x.CheckedPrice).GreaterThanOrEqualTo(0)
                .When(x => x.CheckedPrice.HasValue).WithMessage("Los precios no pueden ser negativos");

            RuleFor(x => x.AvailableSeats).GreaterThanOrEqualTo(0)
                .WithMessage("AvailableSeats no puede ser negativo");

            RuleFor(x => x.ServiceDescription)
                .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.ServiceDescription))
                .WithMessage("La descripcion de servicios no puede superar 100 caracteres");
        }
    }
}