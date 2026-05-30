using System;
using System.Collections.Generic;
using System.Text;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities.External;
using zuli_Repository.Interface;
using zuli_Data.Exceptions;
using zuli_Business.Validation;
using zuli_Business.DTO.External;

namespace zuli_Business
{
    public class FlightService : IFlightService
    {
        // Inyeccion de dependencias
        private readonly IFlightRepository _repository;
        private readonly FlightValidator _validator;
        public FlightService(IFlightRepository repository)
        {
            _repository = repository;
            _validator = new FlightValidator();
        }

        public async Task<IEnumerable<BookedFlightDTO>> RetrieveAvailableFlights(RequestedFlightDTO requestedFlight)
        {
            // El validate requested flight valida cosas
            _validator.ValidateRequestedFlightInfo(requestedFlight);

            var newRequestedFlight = new RequestedFlightEntity
            {
                origin = requestedFlight.origin,
                destination = requestedFlight.destination,
                earliestDeparture = requestedFlight.earliestDeparture,
                latestDeparture = requestedFlight.latestDeparture,
                passengersQuantity = requestedFlight.passengersQuantity,
            };

            var flightArray = await _repository.RetrieveAvailableFlights(newRequestedFlight);

            return flightArray.Select(item => new BookedFlightDTO
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
