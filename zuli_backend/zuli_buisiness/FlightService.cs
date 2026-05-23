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
        private readonly IServiceRepository _serviceRepository;
        private readonly IFlightPathFinder _pathFinder;
        private readonly IFlightSearchMapper _mapper;

        public FlightService(
            IFlightRepository repository,
            IServiceRepository serviceRepository,
            IFlightPathFinder pathFinder,
            IFlightSearchMapper mapper)
        {
            _repository = repository;
            _validator = new FlightValidator();
            _searchValidator = new FlightSearchValidator();
            _serviceRepository = serviceRepository;
            _pathFinder = pathFinder;
            _mapper = mapper;
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
                AircraftId = flight.AircraftId,
                Duration = flight.Duration,
                CarryOnPrice = flight.CarryOnPrice,
                CarryOnWeight = flight.CarryOnWeight,
                CheckedPrice = flight.CheckedPrice,
                CheckedMaxWeight = flight.CheckedMaxWeight,
                CheckedWeightMultiplier = flight.CheckedWeightMultiplier,
                AvailableSeats = flight.AvailableSeats,
                RealArrivalAirport = flight.RealArrivalAirport,
                RealDepartureAirport = flight.RealDepartureAirport,
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
        /*
        public async Task<IEnumerable<FlightDTO>> GetAllFlights()
        {
            var flights = await _repository.GetAllFlights();
            var flightList = flights.ToList();

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
                Duration = f.Duration,
                CarryOnPrice = f.CarryOnPrice,
                CarryOnWeight = f.CarryOnWeight,
                CheckedPrice = f.CheckedPrice,
                CheckedMaxWeight = f.CheckedMaxWeight,
                CheckedWeightMultiplier = f.CheckedWeightMultiplier,
                AvailableSeats = f.AvailableSeats,
                RealArrivalAirport = f.RealArrivalAirport,
                RealDepartureAirport = f.RealDepartureAirport,
                FlightRouteId = f.FlightRouteId,
                ServiceDescription = serviceDescriptions[index]?.Description,
            }).ToList();
        }
        */
        
        public async Task<FlightDTO?> GetFlightById(Guid id)
        {
            var flight = await _repository.GetFlightById(id);

            if (flight == null)
                return null;

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
                AircraftId = flight.AircraftId,
                Duration = flight.Duration,
                CarryOnPrice = flight.CarryOnPrice,
                CarryOnWeight = flight.CarryOnWeight,
                CheckedPrice = flight.CheckedPrice,
                CheckedMaxWeight = flight.CheckedMaxWeight,
                CheckedWeightMultiplier = flight.CheckedWeightMultiplier,
                AvailableSeats = flight.AvailableSeats,
                RealArrivalAirport = flight.RealArrivalAirport,
                RealDepartureAirport = flight.RealDepartureAirport,
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
            await _repository.EnsureFlightsExist(targetDate, request.Seats, targetDate.ToDayOfWeekMask(), DateTime.UtcNow);
            await _repository.EnsureFlightsExist(targetDate.AddDays(1), request.Seats, targetDate.AddDays(1).ToDayOfWeekMask(), DateTime.UtcNow);

            var day1Flights = await _repository.GetAvailableFlights(targetDate, request.Seats, targetDate.ToDayOfWeekMask());
            var day2Flights = await _repository.GetAvailableFlights(targetDate.AddDays(1), request.Seats, targetDate.AddDays(1).ToDayOfWeekMask());

            var rawFlights = day1Flights.Concat(day2Flights).ToList();

            var paths = _pathFinder.FindPaths(rawFlights, origin, destination, targetDate, request.DirectFlightsOnly, request.MaxLayovers);

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
