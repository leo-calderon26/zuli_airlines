using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
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
        public async Task<IEnumerable<RawFlightEntity>> GetAvailableFlights(DateTime earliestDeparture, string destination, int passengersQuantity)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT 
                    fr.FlightRouteId,
                    fr.EstimatedDuration,
                    fr.DepartureAirport AS DepartureAiportCode,
                    depAirport.Name AS DepartureAiportName,
                    depAirport.City AS DepartureCityName,
                    fr.ArrivalAirport AS ArrivalAiportCode,
                    arrAirport.Name AS ArrivalAirportName,
                    arrAirport.City AS ArrivalAirportCity,
                    fr.Frequency,
                    fr.ScheduledDepartureTime AS ScheduledDepartureTime,
                    fr.ScheduledArrivalTime AS ScheduledArrivalTime,
                    fr.TouristPrice,
                    fr.FirstClassPrice,
                    fr.CarryOnPrice,
                    fr.CheckedPrice
                FROM FlightRoute fr
            LEFT JOIN Airport depAirport 
                ON fr.DepartureAirport = depAirport.AirportCode
            LEFT JOIN Airport arrAirport 
                ON fr.ArrivalAirport = arrAirport.AirportCode
            LEFT JOIN Aircraft a 
                ON fr.AircraftId = a.AircraftId
            LEFT JOIN Flight f 
                ON fr.FlightRouteId = f.FlightRouteId
                WHERE 
                    (
                        (f.Id IS NULL AND dbo.getAircraftTotalSeats(a.AircraftId) >= @PassengersQuantity)
                        OR 
                        (f.Id IS NOT NULL AND f.Status != 'Cancelado' AND f.AvailableSeats >= @PassengersQuantity)
                    )
                    AND (fr.ArrivalAirport) = @Destination
                    AND (CAST(CAST(@EarliestDeparture AS DATE) AS DATETIME) + CAST(fr.ScheduledDepartureTime AS DATETIME)) > @CurrentTime";

            var parameters = new
            {
                PassengersQuantity = passengersQuantity,
                Destination = destination,
                EarliestDeparture = earliestDeparture,
                CurrentTime = DateTime.Now
            };

            return await connection.QueryAsync<RawFlightEntity>(sql, parameters);
        }

        public async Task<FlightRouteEntity> GetRouteByFlightId(Guid flightId) {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT
                    fr.FlightRouteId,
                    f.FlightDate AS departureDate
                FROM Flight f
                INNER JOIN FlightRoute fr on f.FlightRouteId = fr.FlightRouteId
                WHERE f.Id = @FlightId";

            var parameters = new
            {
                FlightId = flightId
            };

            return await connection.QuerySingleOrDefaultAsync<FlightRouteEntity>(sql, parameters);
        }

        public async Task<ReservedFlightEntity> GetReservedFlightData(Guid flightId) {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT
                    f.RealDepartureTime,
                    f.RealArrivalTime,
                    f.Duration,
                    f.RealArrivalAirport AS ArrivalAiportCode,
                    aa.Name AS ArrivalAirportName,
                    aa.City AS ArrivalAirportCity,
                    f.RealDepartureAirport AS DepartureAirportCode,
                    da.Name AS DepartureAirportName,
                    da.City AS DepartureCityName,
                    f.TouristPrice,
                    f.FirstClassPrice,
                    f.CarryOnPrice,
                    f.CheckedPrice
                FROM Flight f
                INNER JOIN Airport aa on f.RealArrivalAirport = aa.AirportCode
                INNER JOIN Airport da on f.RealDepartureAirport = da.AirportCode
                WHERE f.Id = @FlightId";

            var parameters = new
            {
                FlightId = flightId
            };

            return await connection.QuerySingleOrDefaultAsync<ReservedFlightEntity>(sql, parameters);
        }
    }   
}
