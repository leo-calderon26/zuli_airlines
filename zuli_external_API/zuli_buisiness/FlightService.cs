using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zuli_Business.DTO;
using zuli_Business.Utils;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Repository.Interface;
using zuli_Data.Exceptions;
using zuli_Business.Validation;
using zuli_Business.DTO;
using MapsterMapper;

namespace zuli_Business
{
    public class FlightService : IFlightService
    {
        private readonly IFlightPathFinder _pathFinder;
        private readonly IFlightDateGenerator _dateGenerator;
        private readonly IFlightRepository _repository;
        private readonly FlightValidator _validator;
        private readonly IMapper _mapper;
        public FlightService(IFlightPathFinder pathFinder, IFlightRepository repository, IFlightDateGenerator dateGenerator,
            IMapper mapper)
        {
            _pathFinder = pathFinder;
            _repository = repository;
            _dateGenerator = dateGenerator;
            _validator = new FlightValidator();
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookedFlightDTO>> RetrieveAvailableFlights(RequestedFlightDTO requestedFlight)
        {
            _validator.ValidateRequestedFlightInfo(requestedFlight);

            List<RawFlightEntity> rawFlights = await FetchAvailableFlights(requestedFlight.earliestDeparture, requestedFlight.destination, requestedFlight.passengersQuantity);

            List<RawFlightEntity> scheduledFlights = _dateGenerator.GenerateOccurrences(rawFlights, requestedFlight.earliestDeparture, requestedFlight.latestDeparture);

            PathFinderParametersDTO criteria = new PathFinderParametersDTO
            {
                FlightPool = scheduledFlights,
                Origin = requestedFlight.origin,
                Destination = requestedFlight.destination,
                EarliestDeparture = requestedFlight.earliestDeparture,
                LatestDeparture = requestedFlight.latestDeparture,
                DirectFlightsOnly = requestedFlight.DirectFlightsOnly,
                MaxLayovers = requestedFlight.MaxLayovers
            };

            // var flightArray = await _repository.RetrieveAvailableFlights(newRequestedFlight);

            List<List<RawFlightEntity>> validPaths = _pathFinder.FindPaths(criteria);
            List<RawFlightEntity> flatFlights = validPaths.SelectMany(path => path).ToList();
            List<BookedFlightDTO> formattedOptions = _mapper.Map<List<BookedFlightDTO>>(flatFlights);

            return formattedOptions;
        }
        private async Task<List<RawFlightEntity>> FetchAvailableFlights(DateTime earliestDeparture, string destination, int passengersQuantity)
        {
            var dayOneFlights = await _repository.GetAvailableFlights(earliestDeparture, destination, passengersQuantity);

            return dayOneFlights.ToList();
        }
    }
}
