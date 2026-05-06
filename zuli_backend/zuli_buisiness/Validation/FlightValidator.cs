using System;
using System.Collections.Generic;
using System.Linq;
using zuli_Business.DTO;
using zuli_Data.Exceptions;

namespace zuli_Business.Validation
{
    public static class FlightValidationFields
    {
        public const string STATUS = "Status";
        public const string FLIGHT_DATE = "FlightDate";
        public const string DURATION = "Duration";
        public const string PRICES = "Prices";
        public const string ENTITIES = "Entities";
        public const string ROUTE = "FlightRouteId";
        public const string AVAILABILITY = "Availability";
    }

    public class FlightValidator
    {
        public void ValidateFlight(FlightDTO flight)
        {
            var errorInfo = new Dictionary<string, List<string>>()
            {
                { FlightValidationFields.STATUS, new List<string>() },
                { FlightValidationFields.FLIGHT_DATE, new List<string>() },
                { FlightValidationFields.DURATION, new List<string>() },
                { FlightValidationFields.PRICES, new List<string>() },
                { FlightValidationFields.ENTITIES, new List<string>() },
                { FlightValidationFields.ROUTE, new List<string>() },
                { FlightValidationFields.AVAILABILITY, new List<string>() },
            };

            var hasError = false;

            if (string.IsNullOrWhiteSpace(flight.Status))
            {
                errorInfo[FlightValidationFields.STATUS].Add("Debe especificar el estado del vuelo");
                hasError = true;
            }

            if (flight.Status?.Length > 20)
            {
                errorInfo[FlightValidationFields.STATUS].Add("El estado no puede superar 20 caracteres");
                hasError = true;
            }

            if (flight.FlightDate == default)
            {
                errorInfo[FlightValidationFields.FLIGHT_DATE].Add("Debe especificar la fecha del vuelo");
                hasError = true;
            }

            if (flight.AirlineId <= 0)
            {
                errorInfo[FlightValidationFields.ENTITIES].Add("Debe especificar una aerolinea valida");
                hasError = true;
            }

            if (flight.AircraftId == Guid.Empty)
            {
                errorInfo[FlightValidationFields.ENTITIES].Add("Debe especificar una aeronave valida");
                hasError = true;
            }

            if (flight.ItineraryId <= 0)
            {
                errorInfo[FlightValidationFields.ENTITIES].Add("Debe especificar un itinerario valido");
                hasError = true;
            }

            if (flight.AdminId == Guid.Empty)
            {
                errorInfo[FlightValidationFields.ENTITIES].Add("Debe especificar un administrador valido");
                hasError = true;
            }

            if (flight.FlightRouteId <= 0)
            {
                errorInfo[FlightValidationFields.ROUTE].Add("Debe especificar una ruta de vuelo valida");
                hasError = true;
            }

            if (flight.Duration <= 0)
            {
                errorInfo[FlightValidationFields.DURATION].Add("La duracion debe ser mayor a cero");
                hasError = true;
            }

            if (flight.FirstClassPrice < 0 || flight.TouristPrice < 0 || (flight.CarryOnPrice.HasValue && flight.CarryOnPrice < 0) || (flight.CheckedPrice.HasValue && flight.CheckedPrice < 0))
            {
                errorInfo[FlightValidationFields.PRICES].Add("Los precios no pueden ser negativos");
                hasError = true;
            }

            if (flight.AvailableSeats < 0)
            {
                errorInfo[FlightValidationFields.ENTITIES].Add("AvailableSeats no puede ser negativo");
                hasError = true;
            }

            if (!flight.Monday && !flight.Tuesday && !flight.Wednesday && !flight.Thursday && !flight.Friday && !flight.Saturday && !flight.Sunday)
            {
                errorInfo[FlightValidationFields.AVAILABILITY].Add("Debe seleccionar al menos un dia de disponibilidad");
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