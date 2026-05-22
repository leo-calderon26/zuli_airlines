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
        private readonly IFlightSearchMapper _flightSearchMapper;
        private readonly FluentValidation.IValidator<FlightDTO> _validator;
        private readonly FluentValidation.IValidator<FlightSearchRequestDTO> _searchValidator;
        private readonly IMapper _mapper;

        public FlightService(
            IFlightRepository repository,
            IUserRepository userRepository,
            IServiceRepository serviceRepository,
            IFlightPathFinder pathFinder,
            IFlightSearchMapper flightSearchMapper,
            FluentValidation.IValidator<FlightDTO> validator,
            FluentValidation.IValidator<FlightSearchRequestDTO> searchValidator,
            IMapper mapper)
        {
            _repository = repository;
            _userRepository = userRepository; 
            _serviceRepository = serviceRepository;
            _pathFinder = pathFinder; 
            _flightSearchMapper = flightSearchMapper;
            _validator = validator; 
            _searchValidator = searchValidator; 
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

            await _repository.CreateFlight(newFlight);

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
            if (flight == null) return null;

            var dto = _mapper.Map<FlightDTO>(flight);
            dto.BusinessId = await _userRepository.GetBusinessId(flight.AdminId);
            dto.ServiceDescription = (await _serviceRepository.GetServiceByFlightId(flight.Id))?.Description;

            return dto;
        }

        
        public async Task<FlightPaginatedResponseDTO> Search(FlightSearchRequestDTO request)
        {
            var validationResult = await _searchValidator.ValidateAsync(request);
            validationResult.ThrowIfInvalid();

            FlightPaginatedResponseDTO response = new FlightPaginatedResponseDTO { CurrentPage = request.Page };

            await AssignDepartureFlights(response, request);

            if (request.IsRoundTrip && request.ReturnDate.HasValue)
            {
                await AssignReturnFlights(response, request);
            }

            return response;
        }

        private async Task AssignDepartureFlights(FlightPaginatedResponseDTO response, FlightSearchRequestDTO request)
        {
            var result = await FindConfiguredRoutes(request.Origin, request.Destination, request.Date, request);
            response.TotalRecordsDeparture = result.TotalRecords;
            response.TotalPagesDeparture = result.TotalPages;
            response.DepartureFlights = result.Flights;
        }

        private async Task AssignReturnFlights(FlightPaginatedResponseDTO response, FlightSearchRequestDTO request)
        {
            if (request.ReturnDate == null)
            {
                return;
            }

            var result = await FindConfiguredRoutes(request.Destination, request.Origin, request.ReturnDate.Value, request);
            response.TotalRecordsReturn = result.TotalRecords;
            response.TotalPagesReturn = result.TotalPages;
            response.ReturnFlights = result.Flights;
        }

        private async Task<(List<FlightSearchResponseDTO> Flights, int TotalRecords, int TotalPages)> FindConfiguredRoutes(
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
            List<FlightSearchResponseDTO> formattedOptions = _flightSearchMapper.MapToOptions(validPaths, request.FlightClass);

            return PaginateResults(formattedOptions, request.Page, request.PageSize);
        }

        private async Task<List<RawFlightEntity>> FetchAvailableFlights(DateTime targetDate, int seats)
        {
            var dayOneFlights = await _repository.GetAvailableFlights(targetDate, seats, targetDate.ToDayOfWeekMask());
            var dayTwoFlights = await _repository.GetAvailableFlights(targetDate.AddDays(NEXT_DAY_OFFSET), seats, targetDate.AddDays(NEXT_DAY_OFFSET).ToDayOfWeekMask());

            return dayOneFlights.Concat(dayTwoFlights).ToList();
        }

        private (List<FlightSearchResponseDTO> Flights, int TotalRecords, int TotalPages) PaginateResults(
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

            return (paginatedFlights, totalRecords, totalPages);
        }
    }
}