using zuli_Business.DTO;
using zuli_Business.Validation.Strategies;
using zuli_Data.Exceptions;

namespace zuli_Business.Validation
{
    public static class FlightRouteAtributes
    {
        public const string FREQUENCY = "Frequency";
        public const string SCHEDULED_ARRIVAL_TIME = "ScheduledArrivalTime";
        public const string SCHEDULED_DEPARTURE_TIME = "ScheduledDepartureTime";
        public const string ESTIMATED_DURATION = "EstimatedDuration";
        public const string BUSINESS_ID = "BusinessId";
        public const string AIRLINE_ID = "AirlineId";
        public const string ARRIVAL_AIRPORT = "ArrivalAirport";
        public const string DEPARTURE_AIRPORT = "DepartureAirport";
        public const int AIRPORT_LENGTH = 3;
    }

    public class FlightRouteValidator : IValidator<FlightRouteDTO>
    {
        private readonly IEnumerable<IValidationStrategy<FlightRouteDTO>> _strategies;

        public FlightRouteValidator(IEnumerable<IValidationStrategy<FlightRouteDTO>> strategies)
        {
            _strategies = strategies;
        }

        public async Task ValidateAsync(FlightRouteDTO flightRoute)
        {
            var context = new ValidationContext();

            foreach (var strategy in _strategies)
                await strategy.ExecuteAsync(flightRoute, context);

            if (context.HasErrors)
                throw new ZuliValidationException(context.Errors);
        }
    }
}
