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

        public FlightRepository(DapperContext context) => _context = context;

        public async Task<IEnumerable<RawFlightEntity>> GetAvailableFlights(DateTime startDate, DateTime endDate, int seats)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
            SELECT 
                f.Id,
                fr.DepartureAirport AS Origin,
                fr.ArrivalAirport AS Destination,
                f.FlightDate AS DepartureTime,
                DATEADD(second, fr.EstimatedDuration, f.FlightDate) AS ArrivalTime,
                fr.EstimatedDuration,
                f.TouristPrice,
                f.FirstClassPrice
            FROM Flight f
            JOIN FlightRoute fr ON f.FlightRouteId = fr.FlightRouteId
            WHERE f.Status = 'Programado' 
              AND f.AvailableSeats >= @Seats
              AND CAST(f.FlightDate AS DATE) >= CAST(@StartDate AS DATE)
              AND CAST(f.FlightDate AS DATE) <= CAST(@EndDate AS DATE)";

            return await connection.QueryAsync<RawFlightEntity>(sql, new { StartDate = startDate, EndDate = endDate, Seats = seats });
        }
    }
}