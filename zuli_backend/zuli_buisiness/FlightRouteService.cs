using AutoMapper;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class FlightRouteService: IFlightRouteService
    {
        private readonly IFlightRouteRepository _flightRouteRepository;
        private readonly IValidator<FlightRouteDTO> _validator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public FlightRouteService(IFlightRouteRepository flightRouterRepository, IValidator<FlightRouteDTO> validator,
            IMapper mapper, IUserRepository userRepository)
        {
            _flightRouteRepository = flightRouterRepository;
            _validator = validator;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<BasicResponseDTO> CreateFlightRouter(FlightRouteDTO flightRoute)
        {
            await _validator.ValidateAsync(flightRoute);
            if (!await _userRepository.IsAdmin(flightRoute.businessId))
            {
                throw new ZuliNotFoundException("El usuario no tiene permisos de administrador.");
            }
            var userId = await _userRepository.GetUserId(flightRoute.businessId);
            var newFlightRoute = _mapper.Map<FlightRouteEntity>(flightRoute);
            newFlightRoute.adminId = userId;

            if (await _flightRouteRepository.AlreadyExistFlightRoute(newFlightRoute))
            {
                throw new ZuliNotFoundException($"Ya existe una ruta con origen '{newFlightRoute.departureAirport}'," +
                    $" destino '{newFlightRoute.arrivalAirport}', frecuencia {newFlightRoute.frequency}");
            }
            await _flightRouteRepository.CreateFlightRouter(newFlightRoute);

            return new BasicResponseDTO
            {
                StatusCode = 200,
                Message = "Se realizo la creaccion de la ruta correctamente",
            };
        }

        public async Task<FlightRoutePaginatedResponseDTO> GetFlightRoutesPaginated(int pageNumber, int pageSize)
        {
            var (flightRoutes, totalCount) = await _flightRouteRepository.GetFlightRoutesPaginated(pageNumber, pageSize);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var flightRouteDtos = flightRoutes.Select(item => new FlightRouteListDTO
            {
                FlightRouteId = item.flightRouteId,
                Frequency = item.frequency,
                ScheduledArrivalTime = item.scheduledArrivalTime,
                ScheduledDepartureTime = item.scheduledDepartureTime,
                EstimatedDuration = item.estimatedDuration,
                AdminId = item.adminId,
                AirlineId = item.airlineId,
                ArrivalAirport = item.arrivalAirport,
                DepartureAirport = item.departureAirport,
                AircraftId = item.aircraftId,
                CarryOnPrice = item.carryOnPrice,
                CheckedPrice = item.checkedPrice,
                TouristPrice = item.touristPrice,
                FirstClassPrice = item.firstClassPrice,
            }).ToList();

            return new FlightRoutePaginatedResponseDTO
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount,
                TotalPages = totalPages,
                Data = flightRouteDtos
            };
        }
    }
}