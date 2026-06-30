using MapsterMapper;
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
        private readonly IUserRepository _userRepository;
        private readonly IValidator<FlightRouteDTO> _validator;

        public FlightRouteService(IFlightRouteRepository flightRouterRepository, IValidator<FlightRouteDTO> validator,
            IUserRepository userRepository)
        {
            _flightRouteRepository = flightRouterRepository;
            _validator = validator;
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

            var newFlightRoute = new FlightRouteEntity
            {
                adminId = userId,
                airlineId = flightRoute.airlineId,
                arrivalAirport = flightRoute.arrivalAirport,
                departureAirport = flightRoute.departureAirport,
                scheduledArrivalTime = flightRoute.scheduledArrivalTime,
                scheduledDepartureTime = flightRoute.scheduledDepartureTime,
                frequency = flightRoute.frequency,
                estimatedDuration = flightRoute.estimatedDuration,
                aircraftId = flightRoute.aircraftId,
                carryOnPrice = flightRoute.carryOnPrice,
                checkedPrice = flightRoute.checkedPrice,
                touristPrice = flightRoute.touristPrice,
                firstClassPrice = flightRoute.firstClassPrice,
                multiplier = flightRoute.multiplier,
            };

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
        public async Task<BasicResponseDTO> DeleteFlightRoute(int flightRouteId)
        {
            // 1 = HardDeleted, 2 = SoftDeleted, 4 = NotFound
            var deletionResult = await _flightRouteRepository.DeleteFlightRoute(flightRouteId);

            switch (deletionResult)
            {
                case 1:
                    return new BasicResponseDTO
                    {
                        StatusCode = 200,
                        Message = "La ruta se eliminó correctamente."
                    };

                case 2:
                    return new BasicResponseDTO
                    {
                        StatusCode = 200,
                        Message = "La ruta tenía compras o reservas asociadas, " +
                                "por lo que se deshabilitó conservando su historial."
                    };

                default: // 4 = NotFound (no existe o ya fue eliminada)
                    throw new ZuliNotFoundException(
                        $"La ruta con ID {flightRouteId} no existe o ya fue eliminada.");
            }
        }
    }
}