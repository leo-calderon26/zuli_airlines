using Dapper;
using zuli_Data;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Repository
{
    public class FlightRepository : IFlightRepository
    {
        private readonly DapperContext _context;

        public FlightRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateFlight(FlightEntity flight)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                INSERT INTO Flight (
                    Id,
                    Status,
                    FlightDate,
                    TouristPrice,
                    FirstClassPrice,
                    RealDepartureTime,
                    RealArrivalTime,
                    AircraftId,
                    RealArrivalAirport,
                    RealDepartureAirport,
                    Duration,
                    CarryOnPrice,
                    CheckedPrice,
                    AvailableSeats,
                    FlightRouteId
                ) VALUES (
                    @Id,
                    @Status,
                    @FlightDate,
                    @TouristPrice,
                    @FirstClassPrice,
                    @RealDepartureTime,
                    @RealArrivalTime,
                    @AircraftId,
                    @RealArrivalAirport,
                    @RealDepartureAirport,
                    @Duration,
                    @CarryOnPrice,
                    @CheckedPrice,
                    @AvailableSeats,
                    @FlightRouteId
                )";

            return await connection.ExecuteAsync(sql, new
            {
                flight.Id,
                flight.Status,
                flight.FlightDate,
                flight.TouristPrice,
                flight.FirstClassPrice,
                flight.RealDepartureTime,
                flight.RealArrivalTime,
                flight.AircraftId,
                flight.RealArrivalAirport,
                flight.RealDepartureAirport,
                flight.Duration,
                flight.CarryOnPrice,
                flight.CheckedPrice,
                flight.AvailableSeats,
                flight.FlightRouteId
            });
        }

        public async Task<IEnumerable<FlightEntity>> GetAllFlights()
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT 
                    f.Id,
                    f.Status,
                    f.FlightDate,
                    f.TouristPrice,
                    f.FirstClassPrice,
                    f.RealDepartureTime,
                    f.RealArrivalTime,
                    f.AircraftId,
                    f.RealArrivalAirport,
                    f.RealDepartureAirport,
                    f.Duration,
                    f.CarryOnPrice,
                    f.CheckedPrice,
                    f.AvailableSeats,
                    f.FlightRouteId,
                    fr.CheckedBagMultiplier
                FROM Flight f
                INNER JOIN FlightRoute fr ON f.FlightRouteId = fr.FlightRouteId";

            return await connection.QueryAsync<FlightEntity>(sql);
        }

        public async Task<FlightEntity?> GetFlightById(Guid id)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT 
                    f.Id,
                    f.Status,
                    f.FlightDate,
                    f.TouristPrice,
                    f.FirstClassPrice,
                    f.RealDepartureTime,
                    f.RealArrivalTime,
                    f.AircraftId,
                    f.RealArrivalAirport,
                    f.RealDepartureAirport,
                    f.Duration,
                    f.CarryOnPrice,
                    f.CheckedPrice,
                    f.AvailableSeats,
                    f.FlightRouteId,
                    fr.CheckedBagMultiplier
                FROM Flight f
                INNER JOIN FlightRoute fr ON f.FlightRouteId = fr.FlightRouteId
                WHERE f.Id = @Id";

                return await connection.QueryFirstOrDefaultAsync<FlightEntity>(sql, new { Id = id });
            }

                public async Task<IEnumerable<RawFlightEntity>> GetAvailableFlights(DateTime targetDate, int seats, int targetDayMask)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT 
                    fr.FlightRouteId,
                    f.Id AS FlightId,
                    fr.DepartureAirport AS Origin,
                    fr.ArrivalAirport AS Destination,
                    CAST(CAST(@TargetDate AS DATE) AS DATETIME) + CAST(fr.ScheduledDepartureTime AS DATETIME) AS DepartureTime,
                    CASE 
                        WHEN fr.ScheduledArrivalTime < fr.ScheduledDepartureTime 
                        THEN DATEADD(day, 1, CAST(CAST(@TargetDate AS DATE) AS DATETIME) + CAST(fr.ScheduledArrivalTime AS DATETIME))
                        ELSE CAST(CAST(@TargetDate AS DATE) AS DATETIME) + CAST(fr.ScheduledArrivalTime AS DATETIME)
                    END AS ArrivalTime,
                    fr.EstimatedDuration,
                    fr.TouristPrice,
                    fr.FirstClassPrice,
                    fr.CarryOnPrice,
                    fr.CheckedPrice,
                    fr.MaxWeightPerBag,
                    fr.CheckedBagMultiplier
                FROM FlightRoute fr
            LEFT JOIN Aircraft a 
                ON fr.AircraftId = a.AircraftId
            LEFT JOIN Flight f 
                ON fr.FlightRouteId = f.FlightRouteId 
                AND CAST(f.FlightDate AS DATE) = CAST(@TargetDate AS DATE)
                WHERE 
                    (fr.Frequency & @TargetDayMask) > 0
                    AND (
                        (f.Id IS NULL AND ((a.NumberEconomyClassRows * a.NumberSeatingRowsEconomy) + (a.NumberFirstClassRows * a.NumberSeatingRowsFirst)) >= @Seats)
                        OR 
                        (f.Id IS NOT NULL AND f.Status != 'Cancelado' AND f.AvailableSeats >= @Seats)
                    )
                    AND (CAST(CAST(@TargetDate AS DATE) AS DATETIME) + CAST(fr.ScheduledDepartureTime AS DATETIME)) > @CurrentTime";

            var parameters = new 
            { 
                TargetDate = targetDate, 
                Seats = seats, 
                TargetDayMask = targetDayMask, 
                CurrentTime = DateTime.UtcNow  
            };

            return await connection.QueryAsync<RawFlightEntity>(sql, parameters);
        }
    }
}