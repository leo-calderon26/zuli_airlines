using System;
using System.Collections.Generic;
using System.Text;
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
        //Inyeccion de la capa
        private readonly IFlightRouteRepository _flightRouteRepository;
        private readonly FlightRouteValidator _validator;

        public FlightRouteService(IFlightRouteRepository flightRouterRepository)
        {
            _flightRouteRepository = flightRouterRepository;
            _validator = new FlightRouteValidator();
        }

        public async Task<BasicResponseDTO> CreateFlightRouter(FlightRouteDTO flightRoute)
        {
            _validator.ValidationFlightRoute(flightRoute);
            if (!await _flightRouteRepository.IsAdmin(flightRoute.businessId))
            {
                throw new ZuliNotFoundException("El usuario no tiene permisos de administrador.");
            }
            var userId = await _flightRouteRepository.GetUserId(flightRoute.businessId);

            var newFlightRoute = new FlightRouteEntity
            {
                adminId = userId,
                airlineId = flightRoute.airlineId,
                arrivalAirport = flightRoute.arrivalAirport,
                departureAirport = flightRoute.departureAirport,
                scheduledArrivalTime = flightRoute.scheduledArrivalTime,
                scheduledDepartureTime = flightRoute.scheduledDepartureTime,
                frequency = flightRoute.frequency,
                estimatedDuration = flightRoute.estimatedDuration
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
    }
}