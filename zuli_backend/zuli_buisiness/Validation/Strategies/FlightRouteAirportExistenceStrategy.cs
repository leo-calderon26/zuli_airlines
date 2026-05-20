using zuli_Business.DTO;
using zuli_Repository.Interface;

namespace zuli_Business.Validation.Strategies
{
    public class FlightRouteAirportExistenceStrategy : IValidationStrategy<FlightRouteDTO>
    {
        private readonly IAirportRepository _airportRepository;

        public FlightRouteAirportExistenceStrategy(IAirportRepository airportRepository)
        {
            _airportRepository = airportRepository;
        }

        public async Task ExecuteAsync(FlightRouteDTO dto, ValidationContext context)
        {
            if (!string.IsNullOrWhiteSpace(dto.departureAirport) &&
                !await _airportRepository.AlreadyExist(dto.departureAirport))
            {
                context.AddError(FlightRouteAtributes.DEPARTURE_AIRPORT,
                    $"El aeropuerto de salida '{dto.departureAirport}' no existe");
            }

            if (!string.IsNullOrWhiteSpace(dto.arrivalAirport) &&
                !await _airportRepository.AlreadyExist(dto.arrivalAirport))
            {
                context.AddError(FlightRouteAtributes.ARRIVAL_AIRPORT,
                    $"El aeropuerto de llegada '{dto.arrivalAirport}' no existe");
            }
        }
    }
}
