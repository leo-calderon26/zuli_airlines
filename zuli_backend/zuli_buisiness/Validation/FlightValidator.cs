using System;
using System.Collections.Generic;
using System.Linq;
using zuli_Buisiness.DTO;
using zuli_Data.Exceptions;

namespace zuli_Buisiness.Validation
{
    public static class FlightAtributes
    {
        public const string AIRCRAFT = "AircraftId";
        public const string ORIGIN = "OriginAirport";
        public const string DESTINATION = "DestinationAirport";
        public const string FREQUENCY = "Frequency";
        public const string TIMES = "Times";
        public const string DURATION = "Duration";
        public const string PRICES = "Prices";
        public const string BAGGAGE = "Baggage";
    }

    public class FlightValidator
    {
        public void ValidateFlight(FlightDTO flight)
        {
            var errorInfo = new Dictionary<string, List<string>>()
            {
                { FlightAtributes.AIRCRAFT, new List<string>() },
                { FlightAtributes.ORIGIN, new List<string>() },
                { FlightAtributes.DESTINATION, new List<string>() },
                { FlightAtributes.FREQUENCY, new List<string>() },
                { FlightAtributes.TIMES, new List<string>() },
                { FlightAtributes.DURATION, new List<string>() },
                { FlightAtributes.PRICES, new List<string>() },
                { FlightAtributes.BAGGAGE, new List<string>() },
            };

            var hasError = false;

            if (flight.aircraftId == Guid.Empty)
            {
                errorInfo[FlightAtributes.AIRCRAFT].Add("Debe seleccionar una aeronave");
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(flight.originAirport))
            {
                errorInfo[FlightAtributes.ORIGIN].Add("Debe seleccionar un aeropuerto de salida");
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(flight.destinationAirport))
            {
                errorInfo[FlightAtributes.DESTINATION].Add("Debe seleccionar un aeropuerto de llegada");
                hasError = true;
            }

            if (!string.IsNullOrWhiteSpace(flight.originAirport) && !string.IsNullOrWhiteSpace(flight.destinationAirport) && flight.originAirport == flight.destinationAirport)
            {
                errorInfo[FlightAtributes.DESTINATION].Add("El aeropuerto de llegada no puede ser igual al de salida");
                hasError = true;
            }

            if (!(flight.monday || flight.tuesday || flight.wednesday || flight.thursday || flight.friday || flight.saturday || flight.sunday))
            {
                errorInfo[FlightAtributes.FREQUENCY].Add("Debe seleccionar al menos un día de frecuencia");
                hasError = true;
            }

            if (flight.departureTime == default || flight.arrivalTime == default)
            {
                errorInfo[FlightAtributes.TIMES].Add("Debe especificar la hora de salida y la hora de llegada");
                hasError = true;
            }

            if (flight.duration <= TimeSpan.Zero)
            {
                errorInfo[FlightAtributes.DURATION].Add("La duración debe ser mayor a cero");
                hasError = true;
            }

            if (flight.firstClassPrice < 0 || flight.touristPrice < 0 || flight.carryOnPrice < 0 || flight.checkedBaggagePrice < 0)
            {
                errorInfo[FlightAtributes.PRICES].Add("Los precios no pueden ser negativos");
                hasError = true;
            }

            if (flight.carryOnWeightKg < 0 || flight.checkedBaggageMaxWeightKg < 0 || flight.checkedBaggageMultiplierPercent < 0)
            {
                errorInfo[FlightAtributes.BAGGAGE].Add("Los valores de equipaje no pueden ser negativos");
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