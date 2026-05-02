using System;
using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using zuli_Buisiness.DTO;
using zuli_Data.Exceptions;

namespace zuli_Buisiness.Validation
{
    public static class FlightRouteAtributes
    {
        public const string FREQUECY = "Frequecy";
        public const string SCHEDULED_ARRIVAL_TIME = "ScheduledArrivalTime";
        public const string SCHEDULED_DEPARTURE_TIME = "ScheduledDepartureTime";
        public const string ESTIMATED_DURATION = "EstimatedDuration";
        public const string AIRLINE_ID = "AirlineId";
        public const string ARRIVAL_AIRPORT = "ArrivalAirport";
        public const string DEPARTURE_AIRPORT = "DepartureAirport";
    }
    public class FlightRouteValidator
    {
        public void ValidationFlightRoute(FlightRouteDTO flightRoute)
        {
            var errorInfo = new Dictionary<string, List<string>>()
            {
                {FlightRouteAtributes.FREQUECY, new List<string>() },
                {FlightRouteAtributes.SCHEDULED_ARRIVAL_TIME, new List<string>() },
                {FlightRouteAtributes.SCHEDULED_DEPARTURE_TIME, new List<string>() },
                {FlightRouteAtributes.ESTIMATED_DURATION, new List<string>() },

                {FlightRouteAtributes.AIRLINE_ID, new List<string>() },
                {FlightRouteAtributes.ARRIVAL_AIRPORT, new List<string>() },
                {FlightRouteAtributes.DEPARTURE_AIRPORT, new List<string>() },
            };
            var IsEmptyInfo = false;
            if (flightRoute.frequecy < 0)
            {
                errorInfo[FlightRouteAtributes.FREQUECY].Add("La frecuencia del vuelo tiene que ser un numero entero positivo");
                IsEmptyInfo = true;
            }
            if (flightRoute.estimatedDuration < 0)
            {
                errorInfo[FlightRouteAtributes.ESTIMATED_DURATION].Add("La duración estimada debe de ser un numero positivo");
                IsEmptyInfo = true;
            }
            if (string.IsNullOrWhiteSpace(flightRoute.arrivalAirport))
            {
                errorInfo[FlightRouteAtributes.ARRIVAL_AIRPORT].Add("Es necesario ingresar el aeropuerto de llegada");
                IsEmptyInfo = true;
            }
            if (string.IsNullOrWhiteSpace(flightRoute.departureAirport))
            {
                errorInfo[FlightRouteAtributes.DEPARTURE_AIRPORT].Add("Es necesario ingresar el aeropuerto de salida");
                IsEmptyInfo = true;
            }

            if (flightRoute.airlineId <= 0)
            {
                errorInfo[FlightRouteAtributes.AIRLINE_ID].Add("El airlineId debe ser un numero entero positivo");
                IsEmptyInfo = true;
            }
            if (IsEmptyInfo)
            {
                throw new ZuliValidationException(errorInfo.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value));
            }
            else
            {
                if (flightRoute.arrivalAirport.Length > 3 || flightRoute.arrivalAirport.Length < 3)
                {
                    errorInfo[FlightRouteAtributes.ARRIVAL_AIRPORT].Add("El nombre del aeropuerto de destino tiene que estar en un rango de 0-3 caracteres");
                }
                if (flightRoute.departureAirport.Length > 3 || flightRoute.departureAirport.Length < 3)
                {
                    errorInfo[FlightRouteAtributes.DEPARTURE_AIRPORT].Add("El nombre del aeropuerto de salida tiene que estar en un rango de 0-3 caracteres");
                }
                if (errorInfo[FlightRouteAtributes.ARRIVAL_AIRPORT].Count > 0 || errorInfo[FlightRouteAtributes.DEPARTURE_AIRPORT].Count > 0)
                    throw new ZuliValidationException(errorInfo.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value));
            }
        }


    }

}
