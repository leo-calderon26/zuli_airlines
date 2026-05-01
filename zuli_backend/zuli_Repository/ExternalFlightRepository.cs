using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using zuli_Data;
using zuli_Data.Entities.External;
using zuli_Repository.Interface;
namespace zuli_Repository
{
    public class ExternalFlightRepository : IExternalFlightRepository
    {
        // Inyeccion de dependencias de la capa Data
        private readonly DapperContext _context;

        public ExternalFlightRepository(DapperContext context) 
        {
            _context = context;
            
        }

        public async Task<IEnumerable<RetrievedFlightEntity>> RetrieveAvailableFlights(RequestedFlightEntity requestedFlight)
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

            var retrievedInformation = await connection.QueryAsync<RetrievedFlightEntity>(selectSql, param:new
            {
                requestedFlight.origin,
                requestedFlight.destination,
                requestedFlight.earliestDeparture,
                requestedFlight.latestDeparture,
                // passengersQuantity = requestedFlight.passengersQuantity,
            });

            return retrievedInformation.Select(item => new RetrievedFlightEntity
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

    }   
}
