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
        private readonly FlightValidator _validator;

        public FlightService(IFlightRepository repository)
        {
            _repository = repository;
            _validator = new FlightValidator();
        }

        public async Task<BasicResponseDTO> CreateFlight(FlightDTO flight)
        {
            _validator.ValidateFlight(flight);

            var newFlight = new FlightEntity
            {
                Id = Guid.NewGuid(),
                Status = flight.Status,
                FlightDate = flight.FlightDate,
                TouristPrice = flight.TouristPrice,
                FirstClassPrice = flight.FirstClassPrice,
                RealDepartureTime = flight.RealDepartureTime,
                RealArrivalTime = flight.RealArrivalTime,
                CheckInStartTime = flight.CheckInStartTime,
                CheckInDeadline = flight.CheckInDeadline,
                AirlineId = flight.AirlineId,
                AircraftId = flight.AircraftId,
                ItineraryId = flight.ItineraryId,
                Duration = flight.Duration,
                CarryOnPrice = flight.CarryOnPrice,
                CheckedPrice = flight.CheckedPrice,
                AvailableSeats = flight.AvailableSeats,
                AdminId = flight.AdminId,
                FlightRouteId = flight.FlightRouteId,
            };

            await _repository.CreateFlight(newFlight);

            return new BasicResponseDTO
            {
                StatusCode = 200,
                Message = "Se realizo la creacion del vuelo correctamente",
            };
        }

        public async Task<IEnumerable<FlightDTO>> GetAllFlights()
        {
            var flights = await _repository.GetAllFlights();

            return flights.Select(f => new FlightDTO
            {
                Id = f.Id,
                Status = f.Status,
                FlightDate = f.FlightDate,
                TouristPrice = f.TouristPrice,
                FirstClassPrice = f.FirstClassPrice,
                RealDepartureTime = f.RealDepartureTime,
                RealArrivalTime = f.RealArrivalTime,
                CheckInStartTime = f.CheckInStartTime,
                CheckInDeadline = f.CheckInDeadline,
                AirlineId = f.AirlineId,
                AircraftId = f.AircraftId,
                ItineraryId = f.ItineraryId,
                Duration = f.Duration,
                CarryOnPrice = f.CarryOnPrice,
                CheckedPrice = f.CheckedPrice,
                AvailableSeats = f.AvailableSeats,
                AdminId = f.AdminId,
                FlightRouteId = f.FlightRouteId,
            }).ToList();
        }

        public async Task<FlightDTO> GetFlightById(Guid id)
        {
            var flight = await _repository.GetFlightById(id);

            if (flight == null)
                return null;

            return new FlightDTO
            {
                Id = flight.Id,
                Status = flight.Status,
                FlightDate = flight.FlightDate,
                TouristPrice = flight.TouristPrice,
                FirstClassPrice = flight.FirstClassPrice,
                RealDepartureTime = flight.RealDepartureTime,
                RealArrivalTime = flight.RealArrivalTime,
                CheckInStartTime = flight.CheckInStartTime,
                CheckInDeadline = flight.CheckInDeadline,
                AirlineId = flight.AirlineId,
                AircraftId = flight.AircraftId,
                ItineraryId = flight.ItineraryId,
                Duration = flight.Duration,
                CarryOnPrice = flight.CarryOnPrice,
                CheckedPrice = flight.CheckedPrice,
                AvailableSeats = flight.AvailableSeats,
                AdminId = flight.AdminId,
                FlightRouteId = flight.FlightRouteId,
            };
        }