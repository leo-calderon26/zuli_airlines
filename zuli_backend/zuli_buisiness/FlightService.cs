using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Business.Utils;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _repository;
        private readonly FlightValidator _validator;
        private readonly FlightSearchValidator _searchValidator;
        private readonly IUserRepository _userRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IFlightPathFinder _pathFinder;
        private readonly IFlightSearchMapper _mapper;

        public FlightService(
            IFlightRepository repository,
            IUserRepository userRepository,
            IServiceRepository serviceRepository,
            IFlightPathFinder pathFinder,
            IFlightSearchMapper mapper)
        {
            _repository = repository;
            _validator = new FlightValidator();
            _searchValidator = new FlightSearchValidator();
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
            _pathFinder = pathFinder;
            _mapper = mapper;
        }

        public async Task<BasicResponseDTO> CreateFlight(FlightDTO flight)
        {
            _validator.ValidateFlight(flight);
            // TOD(you); tiene que validar el compa tiene permisos
            var adminId = await _userRepository.GetUserId(flight.BusinessId);

            var newFlight = new FlightEntity
            {
                Id = Guid.NewGuid(),
                Status = flight.Status,
                FlightDate = flight.FlightDate,
                TouristPrice = flight.TouristPrice,
                FirstClassPrice = flight.FirstClassPrice,
                RealDepartureTime = flight.RealDepartureTime,
                RealArrivalTime = flight.RealArrivalTime,
                AircraftId = flight.AircraftId,
                RealArrivalAirport = flight.RealArrivalAirport,
                RealDepartureAirport = flight.RealDepartureAirport,
                Duration = flight.Duration,
                CarryOnPrice = flight.CarryOnPrice,
                CheckedPrice = flight.CheckedPrice,
                AvailableSeats = flight.AvailableSeats,
                AdminId = adminId,
                FlightRouteId = flight.FlightRouteId,
            };

            await _repository.CreateFlight(newFlight);

            if (!string.IsNullOrWhiteSpace(flight.ServiceDescription))
            {
                var newService = new ServiceEntity
                {
                    FlightId = newFlight.Id,
                    Description = flight.ServiceDescription.Trim()
                };

                await _serviceRepository.CreateService(newService);
            }

            return new BasicResponseDTO
            {
                StatusCode = 200,
                Message = "Se realizo la creacion del vuelo correctamente",
            };
        }

        public async Task<IEnumerable<FlightDTO>> GetAllFlights()
        {
            var flights = await _repository.GetAllFlights();
            var flightList = flights.ToList();
            var businessIds = await Task.WhenAll(
                flightList.Select(f => _userRepository.GetBusinessId(f.AdminId))
            );

            var serviceDescriptions = await Task.WhenAll(
                flightList.Select(f => _serviceRepository.GetServiceByFlightId(f.Id))
            );

            return flightList.Select((f, index) => new FlightDTO
            {
                Id = f.Id,
                Status = f.Status,
                FlightDate = f.FlightDate,
                TouristPrice = f.TouristPrice,
                FirstClassPrice = f.FirstClassPrice,
                RealDepartureTime = f.RealDepartureTime,
                RealArrivalTime = f.RealArrivalTime,
                AircraftId = f.AircraftId,
                RealArrivalAirport = f.RealArrivalAirport,
                RealDepartureAirport = f.RealDepartureAirport,
                Duration = f.Duration,
                CarryOnPrice = f.CarryOnPrice,
                CheckedPrice = f.CheckedPrice,
                AvailableSeats = f.AvailableSeats,
                BusinessId = businessIds[index],
                FlightRouteId = f.FlightRouteId,
                ServiceDescription = serviceDescriptions[index]?.Description,
            }).ToList();
        }

        public async Task<FlightDTO?> GetFlightById(Guid id)
        {
            var flight = await _repository.GetFlightById(id);

            if (flight == null)
                return null;
            
            var businessId = await _userRepository.GetBusinessId(flight.AdminId);
            var service = await _serviceRepository.GetServiceByFlightId(flight.Id);

            return new FlightDTO
            {
                Id = flight.Id,
                Status = flight.Status,
                FlightDate = flight.FlightDate,
                TouristPrice = flight.TouristPrice,
                FirstClassPrice = flight.FirstClassPrice,
                RealDepartureTime = flight.RealDepartureTime,
                RealArrivalTime = flight.RealArrivalTime,
                RealArrivalAirport = flight.RealArrivalAirport,
                RealDepartureAirport = flight.RealDepartureAirport,
                Duration = flight.Duration,
                CarryOnPrice = flight.CarryOnPrice,
                CheckedPrice = flight.CheckedPrice,
                AvailableSeats = flight.AvailableSeats,
                BusinessId = businessId,
                FlightRouteId = flight.FlightRouteId,
                ServiceDescription = service?.Description,
            };
            
        }

        
        public async Task<FlightPaginatedResponseDTO> Search(FlightSearchRequestDTO request)
        {
            _searchValidator.ValidateSearch(request);

            var response = new FlightPaginatedResponseDTO { CurrentPage = request.Page };

            var outboundResult = await ProcessFlightRoutesAsync(request.Origin, request.Destination, request.Date, request);
            response.TotalRecordsOutbound = outboundResult.TotalRecords;
            response.TotalPagesOutbound = outboundResult.TotalPages;
            response.OutboundFlights = outboundResult.Flights;

            if (request.IsRoundTrip && request.ReturnDate.HasValue)
            {
                var returnResult = await ProcessFlightRoutesAsync(request.Destination, request.Origin, request.ReturnDate.Value, request);
                response.TotalRecordsReturn = returnResult.TotalRecords;
                response.TotalPagesReturn = returnResult.TotalPages;
                response.ReturnFlights = returnResult.Flights;
            }

            return response;
        }

        private async Task<(List<FlightSearchResponseDTO> Flights, int TotalRecords, int TotalPages)> ProcessFlightRoutesAsync(
            string origin, string destination, DateTime targetDate, FlightSearchRequestDTO request)
        {
            var day1Flights = await _repository.GetAvailableFlights(targetDate, request.Seats, targetDate.ToDayOfWeekMask());
            var day2Flights = await _repository.GetAvailableFlights(targetDate.AddDays(1), request.Seats, targetDate.AddDays(1).ToDayOfWeekMask());

            var rawFlights = day1Flights.Concat(day2Flights).ToList();

            var paths = _pathFinder.FindPaths(rawFlights, origin, destination, targetDate, request.DirectFlightsOnly);

            var formattedOptions = _mapper.MapToOptions(paths, request.FlightClass);

            int totalRecords = formattedOptions.Count;
            int totalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

            var paginatedFlights = formattedOptions
                .OrderBy(x => x.TotalPrice)
                .ThenBy(x => x.Stops)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return (paginatedFlights, totalRecords, totalPages);
        }
    }
}