using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using zuli_Business.DTO.External;
using zuli_Data.Exceptions;

namespace zuli_Business.Validation
{
    // Esto son tag pasa saber donde esta el error
    public static class FlightAtributes
    {
        public const string ORIGIN = "Origin"; // Código del aeropuerto
        public const string DESTINATION = "Destination"; // Código del aeropuerto
        public const string EARLIESTDEPARTURE = "EarliestDeparture"; // YYYY-MM-DDThh-mm
        public const string LATESTDEPARTURE = "LatestDeparture"; // YYYY-MM-DDThh-mm
        public const string PASSENGERSQUANTITY = "PassengersQuantity"; // Cantidad de pasajeros (es un int)
    }
    public class ExternalFlightValidator
    {
        public void ValidateRequestedFlightInfo(RequestedFlightDTO requestedFlight)
        {
            var errorInfo = new Dictionary<string, List<string>>()
            {
                {FlightAtributes.ORIGIN, new List<string>() },
                {FlightAtributes.DESTINATION, new List<string>() },
                {FlightAtributes.EARLIESTDEPARTURE, new List<string>() },
                {FlightAtributes.LATESTDEPARTURE, new List<string>() },
                {FlightAtributes.PASSENGERSQUANTITY, new List<string>() },
            };

            var IsEmptyInfo = false;
            //Validar que el código de destino solo pueda traer letras 
            if (!MiscValidor.ContainsChars(requestedFlight.origin))
            {
                errorInfo[FlightAtributes.ORIGIN].Add("El código del aeropuerto de origen no puede tener números ni caractes especiales");
                IsEmptyInfo = true;
            }
            //Validar que el código de destino solo pueda traer letras 
            if (!MiscValidor.ContainsChars(requestedFlight.destination))
            {
                errorInfo[FlightAtributes.DESTINATION].Add("El código del aeropuerto de destino no puede tener números ni caractes especiales");
                IsEmptyInfo = true;
            }
            if (requestedFlight.latestDeparture < requestedFlight.earliestDeparture)
            {
                errorInfo[FlightAtributes.EARLIESTDEPARTURE].Add("La fecha más tardía de salida no puede ser antes de la fecha actual");
                IsEmptyInfo = true;
            }
            if (IsEmptyInfo)
            {
                throw new ZuliValidationException(errorInfo.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value));
            }
            else
            {
                if (requestedFlight.origin == requestedFlight.destination) 
                {
                    errorInfo[FlightAtributes.ORIGIN].Add("El código de aeropuerto de origen no puede ser igual al código de aeropuerto de destino");
                }
                if (errorInfo[FlightAtributes.ORIGIN].Count > 0 || errorInfo[FlightAtributes.DESTINATION].Count > 0 || errorInfo[FlightAtributes.PASSENGERSQUANTITY].Count > 0)
                    throw new ZuliValidationException(errorInfo.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value));
            }
        }
    }
}
