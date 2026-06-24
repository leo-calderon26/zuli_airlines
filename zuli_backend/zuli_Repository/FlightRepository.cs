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
            var sql = @"SELECT Id from flight WHERE FlightRouteId = @FlightRouteId AND CAST(FlightDate AS DATE) = @DepartureDate";

            return await connection.QueryFirstOrDefaultAsync<Guid>(sql, new
            return await connection.QueryFirstOrDefaultAsync<Guid>(sql, new
            {
                FlightRouteId = FlightRouteId,
                DepartureDate = DepartureDate
                FlightRouteId = FlightRouteId,
                DepartureDate = DepartureDate
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

        public async Task<IEnumerable<FlightEntity>> GetFlightsByIds(IEnumerable<Guid> ids)
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
                WHERE f.Id IN @Ids";

            return await connection.QueryAsync<FlightEntity>(sql, new { Ids = ids });
        }
            return await connection.QueryFirstOrDefaultAsync<FlightEntity>(sql, new { Id = id });
        }

        public async Task<IEnumerable<FlightEntity>> GetFlightsByIds(IEnumerable<Guid> ids)
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
                WHERE f.Id IN @Ids";

            return await connection.QueryAsync<FlightEntity>(sql, new { Ids = ids });
        }

        public async Task<IEnumerable<RawFlightEntity>> GetAvailableFlights(DateTime targetDate, int seats, int targetDayMask)
        public async Task<IEnumerable<RawFlightEntity>> GetAvailableFlights(DateTime targetDate, int seats, int targetDayMask)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT 
                    fr.FlightRouteId,
                    f.Id AS FlightId,
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
        public async Task<decimal> GetRemainingBaggageCapacity(Guid flightId)
        {
            using var connection = _context.CreateConnection();

            const string sql = @"
                SELECT
                    CAST(a.BaggageCapacity AS DECIMAL(18, 2))
                    - ISNULL(SUM(
                        CASE
                            WHEN LOWER(ISNULL(b.Type, '')) LIKE '%maleta%'
                            OR LOWER(ISNULL(b.Type, '')) LIKE '%checked%'
                            OR LOWER(ISNULL(b.Type, '')) LIKE '%fact%'
                            OR LOWER(ISNULL(b.Type, '')) LIKE '%document%'
                            THEN ISNULL(b.Weight, 0)
                            ELSE 0
                        END
                    ), 0) AS RemainingBaggageCapacity
                FROM dbo.Flight f
                INNER JOIN dbo.Aircraft a
                    ON f.AircraftId = a.AircraftId
                LEFT JOIN dbo.BoardingPass bp
                    ON f.Id = bp.FlightId
                LEFT JOIN dbo.Reservation r
                    ON bp.ReservationCode = r.ReservationCode
                LEFT JOIN dbo.Baggage b
                    ON r.ReservationId = b.ReservationId
                    AND bp.PassengerId = b.PassengerId
                WHERE f.Id = @flightId
                GROUP BY a.BaggageCapacity;
            ";

            return await connection.QuerySingleOrDefaultAsync<decimal>(
                sql,
                new
                {
                    flightId
                }
            );
        }
    }
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
        public async Task<decimal> GetRemainingBaggageCapacity(Guid flightId)
        {
            using var connection = _context.CreateConnection();

            const string sql = @"
                SELECT
                    CAST(a.BaggageCapacity AS DECIMAL(18, 2))
                    - ISNULL(SUM(
                        CASE
                            WHEN LOWER(ISNULL(b.Type, '')) LIKE '%maleta%'
                            OR LOWER(ISNULL(b.Type, '')) LIKE '%checked%'
                            OR LOWER(ISNULL(b.Type, '')) LIKE '%fact%'
                            OR LOWER(ISNULL(b.Type, '')) LIKE '%document%'
                            THEN ISNULL(b.Weight, 0)
                            ELSE 0
                        END
                    ), 0) AS RemainingBaggageCapacity
                FROM dbo.Flight f
                INNER JOIN dbo.Aircraft a
                    ON f.AircraftId = a.AircraftId
                LEFT JOIN dbo.BoardingPass bp
                    ON f.Id = bp.FlightId
                LEFT JOIN dbo.Reservation r
                    ON bp.ReservationCode = r.ReservationCode
                LEFT JOIN dbo.Baggage b
                    ON r.ReservationId = b.ReservationId
                    AND bp.PassengerId = b.PassengerId
                WHERE f.Id = @flightId
                GROUP BY a.BaggageCapacity;
            ";

            return await connection.QuerySingleOrDefaultAsync<decimal>(
                sql,
                new
                {
                    flightId
                }
            );
        }
    }
}
