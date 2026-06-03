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

        public async Task<Guid> CreateFlight(int FlightRouteId, string DepartureDate)
        {
            using var connection = _context.CreateConnection();

            var sql = "dbo.sp_CreateFlight";

            return await connection.ExecuteScalarAsync<Guid>(sql, new
            {
                FlightRouteId,
                DepartureDate
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Guid> GetFlightByRoute(int FlightRouteId, string DepartureDate)
        {
            using var connection = _context.CreateConnection();

            var sql = @"SELECT Id from flight WHERE FlightRouteId = @FlightRouteId AND CAST(FlightDate AS DATE) = @DepartureDate";

            return await connection.QueryFirstOrDefaultAsync<Guid>(sql, new
            {
                FlightRouteId = FlightRouteId,
                DepartureDate = DepartureDate
            });
        }

        public async Task<IEnumerable<FlightEntity>> GetAllFlights()
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT 
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
                FROM Flight";

            return await connection.QueryAsync<FlightEntity>(sql);
        }

        public async Task<FlightEntity?> GetFlightById(Guid id)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT 
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
                FROM Flight
                WHERE Id = @Id";

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
                        (f.Id IS NULL AND dbo.getAircraftTotalSeats(a.AircraftId) >= @Seats)
                        OR 
                        (f.Id IS NOT NULL AND f.Status != 'Cancelado' AND f.AvailableSeats >= @Seats)
                    )
                    AND (CAST(CAST(@TargetDate AS DATE) AS DATETIME) + CAST(fr.ScheduledDepartureTime AS DATETIME)) > @CurrentTime";

            var parameters = new 
            { 
                TargetDate = targetDate, 
                Seats = seats, 
                TargetDayMask = targetDayMask, 
                CurrentTime = DateTime.Now  
            };

            return await connection.QueryAsync<RawFlightEntity>(sql, parameters);
        }
        
        public async Task<int> CheckAvailability(int flightRouteId, DateTime targetDate, int seats)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                SELECT TOP 1 1 
                FROM FlightRoute fr
                LEFT JOIN Aircraft a ON fr.AircraftId = a.AircraftId
                LEFT JOIN Flight f ON fr.FlightRouteId = f.FlightRouteId 
                                AND CAST(f.FlightDate AS DATE) = CAST(@TargetDate AS DATE)
                WHERE fr.FlightRouteId = @FlightRouteId
                AND (
                    (f.Id IS NULL AND dbo.getAircraftTotalSeats(a.AircraftId) >= @Seats)
                    OR 
                    (f.Id IS NOT NULL AND f.Status != 'Cancelado' AND f.AvailableSeats >= @Seats)
                )";
            
            return await connection.QueryFirstOrDefaultAsync<int>(sql, new 
            { 
                FlightRouteId = flightRouteId, 
                TargetDate = targetDate, 
                Seats = seats 
            });
        }
    }
}
