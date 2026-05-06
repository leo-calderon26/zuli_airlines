using System;
using System.Collections.Generic;
using System.Text;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Repository.Interface;
using zuli_Data.Exceptions;
using zuli_Business.Validation;
using System.Globalization;
using System.Linq;

namespace zuli_Business
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _repository;
        private readonly FlightValidator _validator;
        private readonly FlightSearchValidator _searchValidator;

        public FlightService(IFlightRepository repository)
        {
            _repository = repository;
            _validator = new FlightValidator();
            _searchValidator = new FlightSearchValidator();
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
                Monday = flight.Monday,
                Tuesday = flight.Tuesday,
                Wednesday = flight.Wednesday,
                Thursday = flight.Thursday,
                Friday = flight.Friday,
                Saturday = flight.Saturday,
                Sunday = flight.Sunday,
                Wifi = flight.Wifi,
                Entertainment = flight.Entertainment,
                Food = flight.Food,
                SeatSelection = flight.SeatSelection,
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
                Monday = f.Monday,
                Tuesday = f.Tuesday,
                Wednesday = f.Wednesday,
                Thursday = f.Thursday,
                Friday = f.Friday,
                Saturday = f.Saturday,
                Sunday = f.Sunday,
                Wifi = f.Wifi,
                Entertainment = f.Entertainment,
                Food = f.Food,
                SeatSelection = f.SeatSelection,
            }).ToList();
        }

        public async Task<FlightDTO?> GetFlightById(Guid id)
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
                Monday = flight.Monday,
                Tuesday = flight.Tuesday,
                Wednesday = flight.Wednesday,
                Thursday = flight.Thursday,
                Friday = flight.Friday,
                Saturday = flight.Saturday,
                Sunday = flight.Sunday,
                Wifi = flight.Wifi,
                Entertainment = flight.Entertainment,
                Food = flight.Food,
                SeatSelection = flight.SeatSelection,
            };
            
        }

        public async Task<FlightPaginatedResponseDTO> Search(FlightSearchRequestDTO request)
        {

            _searchValidator.ValidateSearch(request);

            int offset = (request.Page - 1) * request.PageSize;

            var (entities, totalCount) = await _repository.SearchFlights(
                request.Origin, request.Destination, request.Date, request.Seats, offset, request.PageSize);

            var dtos = entities.Select(e => new FlightSearchResponseDTO
            {
                Origin = e.Origin,
                Destination = e.Destination,
                Stops = e.Stops,
                LayoverAirports = e.Stops > 0 ? e.LayoverAirports : "Vuelo Directo",
                TotalTouristPrice = e.TotalTouristPrice,
                TotalFirstClassPrice = e.TotalFirstClassPrice,

                DepartureTimeText = e.DepartureTime.ToString("h:mm tt", new CultureInfo("es-ES")).ToLower(),
                ArrivalTimeText = e.ArrivalTime.ToString("h:mm tt", new CultureInfo("es-ES")).ToLower(),

                ArrivalDateText = e.ArrivalTime.Date > e.DepartureTime.Date
                                  ? $"Llega el {e.ArrivalTime.ToString("dd MMM", new CultureInfo("es-ES"))}"
                                  : "",

                TotalDurationText = FormatDuration(e.TotalDurationSeconds)
            }).ToList();

            return new FlightPaginatedResponseDTO
            {
                TotalRecords = totalCount,
                CurrentPage = request.Page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Flights = dtos
            };
        }

        private string FormatDuration(int totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            int totalHours = (int)time.TotalHours;
            return $"{totalHours}H, {time.Minutes}M";
        }
    }
}