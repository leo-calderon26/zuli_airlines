using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                    Id, Status, FlightDate,
                    TouristPrice, FirstClassPrice,
                    RealDepartureTime, RealArrivalTime,
                    AircraftId,
                    Duration,
                    CarryOnPrice, CarryOnWeight,
                    CheckedPrice, CheckedMaxWeight, CheckedWeightMultiplier,
                    AvailableSeats,
                    RealArrivalAirport, RealDepartureAirport,
                    FlightRouteId
                ) VALUES (
                    @Id, @Status, @FlightDate,
                    @TouristPrice, @FirstClassPrice,
                    @RealDepartureTime, @RealArrivalTime,
                    @AircraftId,
                    @Duration,
                    @CarryOnPrice, @CarryOnWeight,
                    @CheckedPrice, @CheckedMaxWeight, @CheckedWeightMultiplier,
                    @AvailableSeats,
                    @RealArrivalAirport, @RealDepartureAirport,
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
                flight.Duration,
                flight.CarryOnPrice,
                flight.CarryOnWeight,
                flight.CheckedPrice,
                flight.CheckedMaxWeight,
                flight.CheckedWeightMultiplier,
                flight.AvailableSeats,
                flight.RealArrivalAirport,
                flight.RealDepartureAirport,
                flight.FlightRouteId
            });
        }

        public async Task<IEnumerable<FlightEntity>> GetAllFlights()
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT 
                    Id, Status, FlightDate,
                    TouristPrice, FirstClassPrice,
                    RealDepartureTime, RealArrivalTime,
                    AircraftId,
                    Duration,
                    CarryOnPrice, CarryOnWeight,
                    CheckedPrice, CheckedMaxWeight, CheckedWeightMultiplier,
                    AvailableSeats,
                    RealArrivalAirport, RealDepartureAirport,
                    FlightRouteId
                FROM Flight";

            return await connection.QueryAsync<FlightEntity>(sql);
        }

        public async Task<FlightEntity?> GetFlightById(Guid id)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT 
                    Id, Status, FlightDate,
                    TouristPrice, FirstClassPrice,
                    RealDepartureTime, RealArrivalTime,
                    AircraftId,
                    Duration,
                    CarryOnPrice, CarryOnWeight,
                    CheckedPrice, CheckedMaxWeight, CheckedWeightMultiplier,
                    AvailableSeats,
                    RealArrivalAirport, RealDepartureAirport,
                    FlightRouteId
                FROM Flight
                WHERE Id = @Id";

                return await connection.QueryFirstOrDefaultAsync<FlightEntity>(sql, new { Id = id });
            }

        public async Task<int> EnsureFlightsExist(DateTime targetDate, int seats, int targetDayMask, DateTime currentTime)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                INSERT INTO Flight (
                    Id, Status, FlightDate,
                    TouristPrice, FirstClassPrice,
                    RealDepartureTime, RealArrivalTime,
                    AircraftId,
                    Duration,
                    CarryOnPrice, CarryOnWeight,
                    CheckedPrice, CheckedMaxWeight, CheckedWeightMultiplier,
                    AvailableSeats,
                    RealArrivalAirport, RealDepartureAirport,
                    FlightRouteId
                )
                SELECT 
                    NEWID(),
                    'Programado',
                    CAST(CAST(@TargetDate AS DATE) AS DATETIME) + CAST(fr.ScheduledDepartureTime AS DATETIME),
                    fr.TouristPrice,
                    fr.FirstClassPrice,
                    NULL,
                    NULL,
                    fr.AircraftId,
                    fr.EstimatedDuration,
                    fr.CarryOnPrice,
                    10.00,
                    fr.CheckedPrice,
                    fr.MaxWeightPerBag,
                    fr.CheckedBagMultiplier,
                    (a.NumberEconomyClassRows * a.NumberSeatingRowsEconomy) + (a.NumberFirstClassRows * a.NumberSeatingRowsFirst),
                    fr.ArrivalAirport,
                    fr.DepartureAirport,
                    fr.FlightRouteId
                FROM FlightRoute fr
                INNER JOIN Aircraft a ON fr.AircraftId = a.AircraftId
                WHERE (fr.Frequency & @TargetDayMask) > 0
                    AND NOT EXISTS (
                        SELECT 1 FROM Flight f
                        WHERE f.FlightRouteId = fr.FlightRouteId
                        AND CAST(f.FlightDate AS DATE) = CAST(@TargetDate AS DATE)
                        AND f.Status != 'Cancelado'
                    )
                    AND (CAST(CAST(@TargetDate AS DATE) AS DATETIME) + CAST(fr.ScheduledDepartureTime AS DATETIME)) > @CurrentTime
                    AND ((a.NumberEconomyClassRows * a.NumberSeatingRowsEconomy) + (a.NumberFirstClassRows * a.NumberSeatingRowsFirst)) >= @Seats";

            return await connection.ExecuteAsync(sql, new { TargetDate = targetDate, Seats = seats, TargetDayMask = targetDayMask, CurrentTime = currentTime });
        }

        public async Task<IEnumerable<RawFlightEntity>> GetAvailableFlights(DateTime targetDate, int seats, int targetDayMask)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
            SELECT 
                fr.FlightRouteId,
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
                fr.FirstClassPrice

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

            return await connection.QueryAsync<RawFlightEntity>(sql, new { TargetDate = targetDate, Seats = seats, TargetDayMask = targetDayMask, CurrentTime = DateTime.UtcNow  });
        }
    }
}
