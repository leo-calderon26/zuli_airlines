using System;
using System.Collections.Generic;
using System.Text;
using zuli_Buisiness.DTO;
using zuli_Buisiness.Interface;
using zuli_Buisiness.Validation;
using zuli_Data.Entites;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;

namespace zuli_Buisiness
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
            //TODO(randy): aqui se deberia de ver si tiene permisos
            var newFlightRoute = new FlightRouteEntity
            {
                adminId = flightRoute.adminId,
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
    }
}
