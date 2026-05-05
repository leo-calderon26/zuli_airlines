using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class FlightRouteRepository: IFlightRouteRepository
    {
        private readonly DapperContext _context;

        public FlightRouteRepository(DapperContext context) => _context = context;
        public async Task<int> CreateFlightRouter(FlightRouteEntity flightRoute)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                  INSERT INTO FlightRoute
                    (adminId, airlineId, arrivalAirport, departureAirport, scheduledArrivalTime, scheduledDeparture, frequency, estimatedDuration)
                  VALUES
                    (@AdminId, @AirlineId, @ArrivalAirport, @DepartureAirport, @ScheduledArrivalTime, @ScheduledDepartureTime, @Frequency, @EstimatedDuration);
            ";

            var newId = await connection.ExecuteScalarAsync<int>(sql, new
            {
                AdminId = flightRoute.adminId,                
                AirlineId = flightRoute.airlineId,              
                ArrivalAirport = flightRoute.arrivalAirport,    
                DepartureAirport = flightRoute.departureAirport,       
                ScheduledArrivalTime = flightRoute.scheduledArrivalTime,
                ScheduledDepartureTime = flightRoute.scheduledDepartureTime,
                Frequency = flightRoute.frequency,
                EstimatedDuration = flightRoute.estimatedDuration 
            });
            return newId;
        }

        public async Task<bool> AlreadyExistFlightRoute(FlightRouteEntity flightRoute)
        {
            using var connection = _context.CreateConnection();

            const string sql = @"
                SELECT COUNT(1)
                FROM FlightRoute
                WHERE arrivalAirport = @arrivalAirport
                AND departureAirport = @departureAirport
                AND frequency = @frequency
                AND scheduledArrivalTime = @scheduledArrivalTime
                AND scheduledDeparture = @scheduledDepartureTime
                AND estimatedDuration = @estimatedDuration";

            var count = await connection.ExecuteScalarAsync<int>(sql, new
            {
                flightRoute.arrivalAirport,
                flightRoute.departureAirport,
                flightRoute.frequency,
                flightRoute.scheduledArrivalTime,
                flightRoute.scheduledDepartureTime,
                flightRoute.estimatedDuration
            });

            return count > 0;
        }
 
    }

}
