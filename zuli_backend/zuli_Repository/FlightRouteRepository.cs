using Dapper;
using System.Linq;
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
 
        public async Task<bool> IsAdmin(string businesId)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
            SELECT CASE WHEN EXISTS (
            SELECT 1
            FROM AirlineUser
            WHERE BusinessId = @BusinessId AND UserRole = 'Administrator'
            ) THEN 1 ELSE 0 END";
            return await connection.ExecuteScalarAsync<bool>(sql, new { BusinessId = businesId });
        }
        public async Task<Guid> GetUserId(string businesId)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
            SELECT UserId
            FROM AirlineUser
            WHERE BusinessId = @BusinessId";
            var userId = await connection.ExecuteScalarAsync<Guid?>(sql, new { BusinessId = businesId });
            return userId ?? Guid.Empty;
        }

        public async Task<(IEnumerable<FlightRouteEntity> flightRoutes, int totalCount)> GetFlightRoutesPaginated(int pageNumber, int pageSize)
        {
            using var connection = _context.CreateConnection();

            var countSql = "SELECT COUNT(1) FROM FlightRoute";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            var offset = (pageNumber - 1) * pageSize;

            var sql = @"
                SELECT
                    flightRouteId,
                    frequency,
                    scheduledArrivalTime,
                    scheduledDeparture AS scheduledDepartureTime,
                    estimatedDuration,
                    adminId,
                    airlineId,
                    arrivalAirport,
                    departureAirport
                FROM FlightRoute
                ORDER BY flightRouteId
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            var flightRoutes = (await connection.QueryAsync<FlightRouteEntity>(sql, new { Offset = offset, PageSize = pageSize })).ToList();
            return (flightRoutes, totalCount);
        }
    }

}
