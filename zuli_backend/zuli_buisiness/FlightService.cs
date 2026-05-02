using zuli_Buisiness.DTO;
using zuli_Buisiness.Interface;
using zuli_Buisiness.Validation;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Buisiness
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _repository;
        private readonly IAirportRepository _airportRepository;
        private readonly FlightValidator _validator;

        public FlightService(IFlightRepository repository, IAirportRepository airportRepository)
        {
            _repository = repository;
            _airportRepository = airportRepository;
            _validator = new FlightValidator();
        }

        public async Task<BasicResponseDTO> CreateFlight(FlightDTO flight)
        {
            _validator.ValidateFlight(flight);

            if (!await _airportRepository.AlreadyExist(flight.originAirport))
            {
                throw new zuli_Data.Exceptions.ZuliNotFoundException($"No se encontro un aeropuerto de salida con el codigo {flight.originAirport}");
            }

            if (!await _airportRepository.AlreadyExist(flight.destinationAirport))
            {
                throw new zuli_Data.Exceptions.ZuliNotFoundException($"No se encontro un aeropuerto de llegada con el codigo {flight.destinationAirport}");
            }

            var newFlight = new FlightEntity
            {
                FlightId = Guid.NewGuid(),
                aircraftId = flight.aircraftId,
                originAirport = flight.originAirport,
                destinationAirport = flight.destinationAirport,
                monday = flight.monday,
                tuesday = flight.tuesday,
                wednesday = flight.wednesday,
                thursday = flight.thursday,
                friday = flight.friday,
                saturday = flight.saturday,
                sunday = flight.sunday,
                departureTime = flight.departureTime,
                arrivalTime = flight.arrivalTime,
                duration = flight.duration,
                firstClassPrice = flight.firstClassPrice,
                touristPrice = flight.touristPrice,
                carryOnPrice = flight.carryOnPrice,
                carryOnWeightKg = flight.carryOnWeightKg,
                checkedBaggagePrice = flight.checkedBaggagePrice,
                checkedBaggageMaxWeightKg = flight.checkedBaggageMaxWeightKg,
                checkedBaggageMultiplierPercent = flight.checkedBaggageMultiplierPercent,
                availableSeats = flight.availableSeats,
                status = flight.status,
                createdAt = DateTime.UtcNow,
            };

            await _repository.CreateFlight(newFlight);

            return new BasicResponseDTO
            {
                StatusCode = 200,
                Message = "Se realizo la creacion del vuelo correctamente",
            };
        }
    }
}