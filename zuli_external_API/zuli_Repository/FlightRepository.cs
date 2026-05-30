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
                CurrentTime = DateTime.UtcNow
            };

            return await connection.QueryAsync<RawFlightEntity>(sql, parameters);
        }
    }   
}
