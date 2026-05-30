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
        // Inyeccion de dependencias de la capa Data
        private readonly DapperContext _context;

        public FlightRepository(DapperContext context) 
        {
            _context = context;
            
        }

        public async Task<IEnumerable<BookedFlightEntity>> RetrieveAvailableFlights(RequestedFlightEntity requestedFlight)
        {
            using var connection = _context.CreateConnection();
            // Insertar la nueva aeronave
            var selectSql = @"
                        SELECT f.Id flightGUID,
	                    f.RealDepartureTime departureTime,
	                    f.RealArrivalTime arrivalTime,
	                    f.Duration duration,
	                    a.AirportCode departureAirportCode,
	                    a.Name departureAirportName,
	                    a.City departureAirportCity,
	                    b.AirportCode arrivalAirportCode,
	                    b.Name arrivalAirportName,
	                    b.City arrivalAirportCity,
	                    f.TouristPrice touristPrice,
	                    f.FirstClassPrice firstClassPrice,
	                    f.CarryOnPrice carryOnPrice,
	                    f.CheckedPrice checkedPrice
	                    FROM flight f INNER JOIN Airport a ON f.DepartureAirport = a.AirportCode INNER JOIN Airport b ON f.ArrivalAirport = b.AirportCode
	                    WHERE f.DepartureAirport = @origin AND f.ArrivalAirport = @destination AND f.RealDepartureTime between @earliestDeparture AND @latestDeparture";

            var retrievedInformation = await connection.QueryAsync<BookedFlightEntity>(selectSql, param:new
            {
                requestedFlight.origin,
                requestedFlight.destination,
                requestedFlight.earliestDeparture,
                requestedFlight.latestDeparture,
                // passengersQuantity = requestedFlight.passengersQuantity,
            });

            return retrievedInformation.Select(item => new BookedFlightEntity
            {
                flightGUID = item.flightGUID,
                departureTime = item.departureTime,
                arrivalTime = item.arrivalTime,
                duration = item.duration,
                departureAirportCode = item.departureAirportCode,
                departureAirportName = item.departureAirportName,
                departureAirportCity = item.departureAirportCity,
                arrivalAirportCode = item.arrivalAirportCode,
                arrivalAirportName = item.arrivalAirportName,
                arrivalAirportCity = item.arrivalAirportCity,
                touristPrice = item.touristPrice,
                firstClassPrice = item.firstClassPrice,
                carryOnPrice = item.carryOnPrice,
                checkedPrice = item.checkedPrice
            }).ToList();
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
                        (f.Id IS NULL AND ((a.NumberEconomyClassRows * a.NumberSeatingRowsEconomy) + (a.NumberFirstClassRows * a.NumberSeatingRowsFirst)) >= @PassengersQuantity)
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
