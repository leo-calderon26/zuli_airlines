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
                    (adminId, airlineId, arrivalAirport, departureAirport,
                     scheduledArrivalTime, scheduledDepartureTime, frequency, estimatedDuration,
                     aircraftId, carryOnPrice, checkedPrice, touristPrice, firstClassPrice,
                     checkedBagMultiplier, maxWeightPerBag)
                  VALUES
                    (@AdminId, @AirlineId, @ArrivalAirport, @DepartureAirport,
                     @ScheduledArrivalTime, @ScheduledDepartureTime, @Frequency, @EstimatedDuration,
                     CONVERT(uniqueidentifier, @AircraftId), @CarryOnPrice, @CheckedPrice, @TouristPrice, @FirstClassPrice,
                     @CheckedBagMultiplier, @MaxWeightPerBag);
            ";

            var newId = await connection.ExecuteScalarAsync<int>(sql, new
            {
                flightRoute.adminId,
                flightRoute.airlineId,
                flightRoute.arrivalAirport,
                flightRoute.departureAirport,
                flightRoute.scheduledArrivalTime,
                flightRoute.scheduledDepartureTime,
                flightRoute.frequency,
                flightRoute.estimatedDuration,
                flightRoute.aircraftId,
                flightRoute.carryOnPrice,
                flightRoute.checkedPrice,
                flightRoute.touristPrice,
                flightRoute.firstClassPrice,
                flightRoute.checkedBagMultiplier,
                flightRoute.maxWeightPerBag
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
                AND scheduledDepartureTime = @scheduledDepartureTime
                AND estimatedDuration = @estimatedDuration
                AND aircraftId = CONVERT(uniqueidentifier, @aircraftId)
                AND carryOnPrice = @carryOnPrice
                AND checkedPrice = @checkedPrice
                AND touristPrice = @touristPrice
                AND firstClassPrice = @firstClassPrice
                AND checkedBagMultiplier = @checkedBagMultiplier
                AND maxWeightPerBag = @maxWeightPerBag";

            var count = await connection.ExecuteScalarAsync<int>(sql, new
            {
                flightRoute.arrivalAirport,
                flightRoute.departureAirport,
                flightRoute.frequency,
                flightRoute.scheduledArrivalTime,
                flightRoute.scheduledDepartureTime,
                flightRoute.estimatedDuration,
                flightRoute.aircraftId,
                flightRoute.carryOnPrice,
                flightRoute.checkedPrice,
                flightRoute.touristPrice,
                flightRoute.firstClassPrice,
                flightRoute.checkedBagMultiplier,
                flightRoute.maxWeightPerBag
            });

            return count > 0;
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
                    scheduledDepartureTime,
                    estimatedDuration,
                    adminId,
                    airlineId,
                    arrivalAirport,
                    departureAirport,
                    CAST(aircraftId AS nvarchar(36)) AS aircraftId,
                    carryOnPrice,
                    checkedPrice,
                    touristPrice,
                    firstClassPrice,
                    checkedBagMultiplier,
                    maxWeightPerBag
                FROM FlightRoute
                ORDER BY flightRouteId
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            var flightRoutes = (await connection.QueryAsync<FlightRouteEntity>(sql, new { Offset = offset, PageSize = pageSize })).ToList();
            return (flightRoutes, totalCount);
        }
    }

}
