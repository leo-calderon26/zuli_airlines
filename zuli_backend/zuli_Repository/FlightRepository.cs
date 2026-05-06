using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    CheckInStartTime,
                    CheckInDeadline,
                    AirlineId,
                    AircraftId,
                    ItineraryId,
                    Duration,
                    CarryOnPrice,
                    CheckedPrice,
                    AvailableSeats,
                    AdminId,
                    FlightRouteId
                ) VALUES (
                    @Id,
                    @Status,
                    @FlightDate,
                    @TouristPrice,
                    @FirstClassPrice,
                    @RealDepartureTime,
                    @RealArrivalTime,
                    @CheckInStartTime,
                    @CheckInDeadline,
                    @AirlineId,
                    @AircraftId,
                    @ItineraryId,
                    @Duration,
                    @CarryOnPrice,
                    @CheckedPrice,
                    @AvailableSeats,
                    @AdminId,
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
                flight.CheckInStartTime,
                flight.CheckInDeadline,
                flight.AirlineId,
                flight.AircraftId,
                flight.ItineraryId,
                flight.Duration,
                flight.CarryOnPrice,
                flight.CheckedPrice,
                flight.AvailableSeats,
                flight.AdminId,
                flight.FlightRouteId
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
                    CheckInStartTime,
                    CheckInDeadline,
                    AirlineId,
                    AircraftId,
                    ItineraryId,
                    Duration,
                    CarryOnPrice,
                    CheckedPrice,
                    AvailableSeats,
                    AdminId,
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
                    CheckInStartTime,
                    CheckInDeadline,
                    AirlineId,
                    AircraftId,
                    ItineraryId,
                    Duration,
                    CarryOnPrice,
                    CheckedPrice,
                    AvailableSeats,
                    AdminId,
                    FlightRouteId
                FROM Flight
                WHERE Id = @Id";

                return await connection.QueryFirstOrDefaultAsync<FlightEntity>(sql, new { Id = id });
            }

        public async Task<(IEnumerable<FlightSearchEntity> Flights, int TotalCount)> SearchFlights(
            string origin, string destination, DateTime date, int seats, int offset, int fetch)
        {
            using var connection = _context.CreateConnection();

            var parameters = new 
            { 
                Origin = origin, 
                Destination = destination, 
                Date = date, 
                Seats = seats, 
                Offset = offset, 
                Fetch = fetch 
            };


            var countSql = @"
            DECLARE @DirectFlights INT;
            DECLARE @FlightsWithStop INT;

            SELECT @DirectFlights = COUNT(*)
            FROM Flight f
            INNER JOIN FlightRoute fr ON f.FlightRouteId = fr.FlightRouteId
            WHERE fr.DepartureAirport = @Origin
            AND fr.ArrivalAirport = @Destination
            AND f.AvailableSeats >= @Seats
            AND f.Status = 'Programado'
            AND CAST(f.FlightDate AS DATE) = CAST(@Date AS DATE);

            SELECT @FlightsWithStop = COUNT(*)
            FROM Flight f1
            INNER JOIN FlightRoute fr1 ON f1.FlightRouteId = fr1.FlightRouteId
            INNER JOIN FlightRoute fr2 ON fr1.ArrivalAirport = fr2.DepartureAirport
            INNER JOIN Flight f2 ON f2.FlightRouteId = fr2.FlightRouteId
            WHERE fr1.DepartureAirport = @Origin
            AND fr2.ArrivalAirport = @Destination
            AND f1.AvailableSeats >= @Seats
            AND f2.AvailableSeats >= @Seats
            AND f1.Status = 'Programado'
            AND f2.Status = 'Programado'
            AND CAST(f1.FlightDate AS DATE) = CAST(@Date AS DATE)
            AND f2.FlightDate >= DATEADD(MINUTE, 60, DATEADD(SECOND, fr1.EstimatedDuration, f1.FlightDate))
            AND CAST(f2.FlightDate AS DATE) <= DATEADD(DAY, 1, CAST(@Date AS DATE));

            SELECT @DirectFlights + @FlightsWithStop AS TotalFlights;";

            int totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

            if (totalCount == 0)
            {
                return (new List<FlightSearchEntity>(), 0);
            }

            var dataSql = @"
            SELECT 
                fr1.DepartureAirport AS Origin,
                fr1.ArrivalAirport AS Destination,
                f1.FlightDate AS DepartureTime,
                DATEADD(second, fr1.EstimatedDuration, f1.FlightDate) AS ArrivalTime,
                DATEDIFF(second, f1.FlightDate, DATEADD(second, fr1.EstimatedDuration, f1.FlightDate)) AS TotalDurationSeconds,
                CAST(f1.TouristPrice AS DECIMAL(20,2)) AS TotalTouristPrice,
                CAST(f1.FirstClassPrice AS DECIMAL(20,2)) AS TotalFirstClassPrice,
                0 AS Stops,
                '' AS LayoverAirports
            FROM Flight f1
            JOIN FlightRoute fr1 ON f1.FlightRouteId = fr1.FlightRouteId
            WHERE fr1.DepartureAirport = @Origin AND fr1.ArrivalAirport = @Destination
              AND CAST(f1.FlightDate AS DATE) = CAST(@Date AS DATE)
              AND f1.AvailableSeats >= @Seats AND f1.Status = 'Programado'

            UNION ALL

            SELECT 
                fr1.DepartureAirport AS Origin,
                fr2.ArrivalAirport AS Destination,
                f1.FlightDate AS DepartureTime,
                DATEADD(second, fr2.EstimatedDuration, f2.FlightDate) AS ArrivalTime,
                DATEDIFF(second, f1.FlightDate, DATEADD(second, fr2.EstimatedDuration, f2.FlightDate)) AS TotalDurationSeconds,
                CAST(f1.TouristPrice + f2.TouristPrice AS DECIMAL(20,2)) AS TotalTouristPrice,
                CAST(f1.FirstClassPrice + f2.FirstClassPrice AS DECIMAL(20,2)) AS TotalFirstClassPrice,
                1 AS Stops,
                fr1.ArrivalAirport AS LayoverAirports
            FROM Flight f1
            JOIN FlightRoute fr1 ON f1.FlightRouteId = fr1.FlightRouteId
            JOIN FlightRoute fr2 ON fr1.ArrivalAirport = fr2.DepartureAirport
            JOIN Flight f2 ON f2.FlightRouteId = fr2.FlightRouteId
            WHERE fr1.DepartureAirport = @Origin AND fr2.ArrivalAirport = @Destination
              AND CAST(f1.FlightDate AS DATE) = CAST(@Date AS DATE)
              AND f1.AvailableSeats >= @Seats AND f2.AvailableSeats >= @Seats
              AND f1.Status = 'Programado' AND f2.Status = 'Programado'
              AND f2.FlightDate >= DATEADD(minute, 60, DATEADD(second, fr1.EstimatedDuration, f1.FlightDate))
              AND CAST(f2.FlightDate AS DATE) <= DATEADD(day, 1, CAST(@Date AS DATE))

            ORDER BY TotalTouristPrice ASC, TotalDurationSeconds ASC
            OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY;";

            var flights = await connection.QueryAsync<FlightSearchEntity>(dataSql, parameters);

            return (flights, totalCount);
        }
    }
}