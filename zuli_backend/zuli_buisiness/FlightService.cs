using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MapsterMapper;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Business.Utils;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Repository.Interface;
using zuli_Data.Exceptions;

namespace zuli_Business
{
    public class FlightService : IFlightService
    {
        private const int SUCCESS_STATUS_CODE = 200;
        private const int NEXT_DAY_OFFSET = 1;

        private readonly IFlightRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IFlightPathFinder _pathFinder;
        private readonly FluentValidation.IValidator<FlightDTO> _validator;
        private readonly FluentValidation.IValidator<FlightSearchRequestDTO> _searchValidator;
        private readonly FluentValidation.IValidator<FlightAvailabilityRequestDTO> _availabilityValidator;
        private readonly IMapper _mapper;

        public FlightService(
            IFlightRepository repository,
            IUserRepository userRepository,
            IServiceRepository serviceRepository,
            IFlightPathFinder pathFinder,
            FluentValidation.IValidator<FlightDTO> validator,
            FluentValidation.IValidator<FlightSearchRequestDTO> searchValidator,
            FluentValidation.IValidator<FlightAvailabilityRequestDTO> availabilityValidator,
            IMapper mapper)
        {
            _repository = repository;
            _userRepository = userRepository; 
            _serviceRepository = serviceRepository;
            _pathFinder = pathFinder; 
            _validator = validator; 
            _searchValidator = searchValidator; 
            _availabilityValidator = availabilityValidator;
            _mapper = mapper;
        }

        public async Task<BasicResponseDTO> CreateFlight(FlightDTO flight)
        {
            var validationResult = await _validator.ValidateAsync(flight);
            validationResult.ThrowIfInvalid();

            var adminId = await _userRepository.GetUserId(flight.BusinessId);
            var newFlight = _mapper.Map<FlightEntity>(flight);
            newFlight.Id = Guid.NewGuid();
            newFlight.AdminId = adminId;

            // await _repository.CreateFlight(newFlight);

            if (!string.IsNullOrWhiteSpace(flight.ServiceDescription))
            {
                await _serviceRepository.CreateService(new ServiceEntity
                {
                    FlightId = newFlight.Id,
                    Description = flight.ServiceDescription.Trim()
                });
            }

            return new BasicResponseDTO 
            { 
                StatusCode = SUCCESS_STATUS_CODE, 
                Message = "Se realizo la creacion del vuelo correctamente",
                };
        }

        public async Task<IEnumerable<FlightDTO>> GetAllFlights()
        {
            var flights = (await _repository.GetAllFlights()).ToList();
            var businessIds = await Task.WhenAll(flights.Select(f => _userRepository.GetBusinessId(f.AdminId)));
            var services = await Task.WhenAll(flights.Select(f => _serviceRepository.GetServiceByFlightId(f.Id)));
            var result = _mapper.Map<List<FlightDTO>>(flights);
            
            for (int i = 0; i < result.Count; i++)
            {
                result[i].BusinessId = businessIds[i];
                result[i].ServiceDescription = services[i]?.Description;
            }
            return result;
        }

        public async Task<FlightDTO?> GetFlightById(Guid id)
        {
            var flight = await _repository.GetFlightById(id);
            if (flight == null)
            {
            throw new ZuliNotFoundException($"No se encontró el vuelo con el identificador {id}");
            }

            var dto = _mapper.Map<FlightDTO>(flight);
            dto.BusinessId = await _userRepository.GetBusinessId(flight.AdminId);
            dto.ServiceDescription = (await _serviceRepository.GetServiceByFlightId(flight.Id))?.Description;

            return dto;
        }

        
        public async Task<FlightPaginatedResponseDTO> Search(FlightSearchRequestDTO request)
        {
            var validationResult = await _searchValidator.ValidateAsync(request);
            validationResult.ThrowIfInvalid();

            var response = new FlightPaginatedResponseDTO { CurrentPage = request.Page };

            var departureRoutes = await FindConfiguredRoutes(request.Origin, request.Destination, request.Date, request);
            
            response.TotalRecordsDeparture = departureRoutes.TotalRecords;
            response.TotalPagesDeparture = departureRoutes.TotalPages;
            response.DepartureFlights = departureRoutes.Flights;

            if (request.IsRoundTrip && request.ReturnDate.HasValue)
            {
                var returnRoutes = await FindConfiguredRoutes(request.Destination, request.Origin, request.ReturnDate.Value, request);
                
                response.TotalRecordsReturn = returnRoutes.TotalRecords;
                response.TotalPagesReturn = returnRoutes.TotalPages;
                response.ReturnFlights = returnRoutes.Flights;
            }

            return response;
        }

        private async Task<PaginatedFlightListDTO> FindConfiguredRoutes(
            string origin, string destination, DateTime targetDate, FlightSearchRequestDTO request)
        {
            List<RawFlightEntity> rawFlights = await FetchAvailableFlights(targetDate, request.Seats);

            PathFinderParametersDTO criteria = new PathFinderParametersDTO
            {
                FlightPool = rawFlights,
                Origin = origin,
                Destination = destination,
                TargetDate = targetDate,
                DirectFlightsOnly = request.DirectFlightsOnly,
                MaxLayovers = request.MaxLayovers
            };

            List<List<RawFlightEntity>> validPaths = _pathFinder.FindPaths(criteria);
            List<FlightSearchResponseDTO> formattedOptions = _mapper.Map<List<FlightSearchResponseDTO>>(validPaths);
            return PaginateResults(formattedOptions, request.Page, request.PageSize);
        }

        private async Task<List<RawFlightEntity>> FetchAvailableFlights(DateTime targetDate, int seats)
        {
            var dayOneFlights = await _repository.GetAvailableFlights(targetDate, seats, targetDate.ToDayOfWeekMask());
            var dayTwoFlights = await _repository.GetAvailableFlights(targetDate.AddDays(NEXT_DAY_OFFSET), seats, targetDate.AddDays(NEXT_DAY_OFFSET).ToDayOfWeekMask());

            return dayOneFlights.Concat(dayTwoFlights).ToList();
        }

        private PaginatedFlightListDTO PaginateResults(
            List<FlightSearchResponseDTO> allOptions, int page, int pageSize)
        {
            int totalRecords = allOptions.Count;
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            int skipAmount = (page - 1) * pageSize;

            List<FlightSearchResponseDTO> paginatedFlights = allOptions
                .OrderBy(option => option.TotalTouristPrice)
                .ThenBy(option => option.Stops)
                .Skip(skipAmount)
                .Take(pageSize)
                .ToList();

            return new PaginatedFlightListDTO
            {
                Flights = paginatedFlights,
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };
        }

        public async Task<BasicResponseDTO> CheckAvailability(FlightAvailabilityRequestDTO request)
        {
            var validationResult = await _availabilityValidator.ValidateAsync(request);
            validationResult.ThrowIfInvalid();

            foreach (var segment in request.Segments)
            {
                int availabilityStatus = await _repository.
                    CheckAvailability(segment.FlightRouteId, segment.DepartureDate, request.Seats);
                if (availabilityStatus == 0)
                {
                    throw new ZuliValidationException("Seats", "No hay espacios disponibles en el trayecto seleccionado.");
                }
            }

            return new BasicResponseDTO 
            { 
                StatusCode = SUCCESS_STATUS_CODE, 
                Message = "Asientos disponibles" 
            };
        }
    }
}