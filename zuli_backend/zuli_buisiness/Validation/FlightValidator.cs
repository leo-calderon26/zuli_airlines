using System;
using System.Collections.Generic;
using System.Linq;
using zuli_Buisiness.DTO;
using zuli_Data.Exceptions;

namespace zuli_Buisiness.Validation
{
    public static class FlightAtributes
    {
        public const string STATUS = "Status";
        public const string FLIGHT_DATE = "FlightDate";
        public const string DURATION = "Duration";
        public const string PRICES = "Prices";
        public const string ENTITIES = "Entities";
        public const string ROUTE = "FlightRouteId";
    }

    public class FlightValidator
    {
        public void ValidateFlight(FlightDTO flight)
        {
            var errorInfo = new Dictionary<string, List<string>>()
            {
                { FlightAtributes.STATUS, new List<string>() },
                { FlightAtributes.FLIGHT_DATE, new List<string>() },
                { FlightAtributes.DURATION, new List<string>() },
                { FlightAtributes.PRICES, new List<string>() },
                { FlightAtributes.ENTITIES, new List<string>() },
                { FlightAtributes.ROUTE, new List<string>() },
            };

            var hasError = false;

            if (string.IsNullOrWhiteSpace(flight.Status))
            {
                errorInfo[FlightAtributes.STATUS].Add("Debe especificar el estado del vuelo");
                hasError = true;
            }

            if (flight.Status?.Length > 20)
            {
                errorInfo[FlightAtributes.STATUS].Add("El estado no puede superar 20 caracteres");
                hasError = true;
            }

            if (flight.FlightDate == default)
            {
                errorInfo[FlightAtributes.FLIGHT_DATE].Add("Debe especificar la fecha del vuelo");
                hasError = true;
            }

            if (flight.AirlineId <= 0)
            {
                errorInfo[FlightAtributes.ENTITIES].Add("Debe especificar una aerolinea valida");
                hasError = true;
            }

            if (flight.AircraftId == Guid.Empty)
            {
                errorInfo[FlightAtributes.ENTITIES].Add("Debe especificar una aeronave valida");
                hasError = true;
            }

            if (flight.ItineraryId <= 0)
            {
                errorInfo[FlightAtributes.ENTITIES].Add("Debe especificar un itinerario valido");
                hasError = true;
            }

            if (flight.AdminId == Guid.Empty)
            {
                errorInfo[FlightAtributes.ENTITIES].Add("Debe especificar un administrador valido");
                hasError = true;
            }

            if (flight.FlightRouteId <= 0)
            {
                errorInfo[FlightAtributes.ROUTE].Add("Debe especificar una ruta de vuelo valida");
                hasError = true;
            }

            if (flight.Duration <= 0)
            {
                errorInfo[FlightAtributes.DURATION].Add("La duracion debe ser mayor a cero");
                hasError = true;
            }

            if (flight.FirstClassPrice < 0 || flight.TouristPrice < 0 || (flight.CarryOnPrice.HasValue && flight.CarryOnPrice < 0) || (flight.CheckedPrice.HasValue && flight.CheckedPrice < 0))
            {
                errorInfo[FlightAtributes.PRICES].Add("Los precios no pueden ser negativos");
                hasError = true;
            }

            if (hasError)
            {
                throw new ZuliValidationException(
                    errorInfo.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value)
                );
            }
        }
    }
}