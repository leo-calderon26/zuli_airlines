using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zuli_Business.DTO;
using zuli_Data.Exceptions;

namespace zuli_Business.Validation
{
    public static class AirportAtributes
    {
        public const string CODE = "AirportCode";
        public const string NAME = "Name";
        public const string COUNTRY = "Country";
        public const string CITY = "City";
        public const string ADMINID = "AdminId";
    }

    public class AirportValidator
    {
        public void ValidateAirportInfo(AirportDTO airport)
        {
            var errorInfo = new Dictionary<string, List<string>>()
            {
                { AirportAtributes.CODE, new List<string>() },
                { AirportAtributes.NAME, new List<string>() },
                { AirportAtributes.COUNTRY, new List<string>() },
                { AirportAtributes.CITY, new List<string>() },
                { AirportAtributes.ADMINID, new List<string>() },
            };

            var IsEmptyInfo = false;

            // Validar si el codigo no viene vacio
            if (string.IsNullOrWhiteSpace(airport.airportCode))
            {
                errorInfo[AirportAtributes.CODE].Add("El codigo del aeropuerto no puede venir vacio o con espacios en blanco");
                IsEmptyInfo = true;
            }

            // Validar si el nombre no viene vacio
            if (string.IsNullOrWhiteSpace(airport.name))
            {
                errorInfo[AirportAtributes.NAME].Add("El nombre del aeropuerto no puede venir vacio o con espacios en blanco");
                IsEmptyInfo = true;
            }

            // Validar si el pais no viene vacio
            if (string.IsNullOrWhiteSpace(airport.country))
            {
                errorInfo[AirportAtributes.COUNTRY].Add("El pais no puede venir vacio o con espacios en blanco");
                IsEmptyInfo = true;
            }

            // Validar si la ciudad no viene vacia
            if (string.IsNullOrWhiteSpace(airport.city))
            {
                errorInfo[AirportAtributes.CITY].Add("La ciudad no puede venir vacia o con espacios en blanco");
                IsEmptyInfo = true;
            }

            // Validar que el adminId no venga vacio
            if (airport.adminId == Guid.Empty)
            {
                errorInfo[AirportAtributes.ADMINID].Add("El adminId no puede venir vacio");
                IsEmptyInfo = true;
            }

            if (IsEmptyInfo)
            {
                throw new ZuliValidationException(
                    errorInfo.Where(x => x.Value.Count > 0)
                             .ToDictionary(x => x.Key, x => x.Value)
                );
            }
            else
            {
                // Validar longitud exacta del codigo del aeropuerto
                if (airport.airportCode.Length != 3)
                {
                    errorInfo[AirportAtributes.CODE].Add("El codigo del aeropuerto debe tener exactamente 3 caracteres");
                    IsEmptyInfo = true;
                }

                // Validar que el codigo no tenga espacios
                if (airport.airportCode.Contains(" "))
                {
                    errorInfo[AirportAtributes.CODE].Add("El codigo del aeropuerto no puede contener espacios");
                    IsEmptyInfo = true;
                }

                // Validar que el codigo solo tenga letras
                if (!airport.airportCode.All(char.IsLetter))
                {
                    errorInfo[AirportAtributes.CODE].Add("El codigo del aeropuerto solo puede contener letras");
                    IsEmptyInfo = true;
                }

                // Validar longitud maxima del nombre
                if (!string.IsNullOrWhiteSpace(airport.name) && airport.name.Length > 100)
                {
                    errorInfo[AirportAtributes.NAME].Add("El nombre del aeropuerto no puede medir más de 100 caracteres");
                    IsEmptyInfo = true;
                }

                // Validar longitud maxima del pais
                if (!string.IsNullOrWhiteSpace(airport.country) && airport.country.Length > 60)
                {
                    errorInfo[AirportAtributes.COUNTRY].Add("El pais no puede medir más de 60 caracteres");
                    IsEmptyInfo = true;
                }

                // Validar longitud maxima de la ciudad
                if (!string.IsNullOrWhiteSpace(airport.city) && airport.city.Length > 60)
                {
                    errorInfo[AirportAtributes.CITY].Add("La ciudad no puede medir más de 60 caracteres");
                    IsEmptyInfo = true;
                }

                if (IsEmptyInfo)
                {
                    throw new ZuliValidationException(
                        errorInfo.Where(x => x.Value.Count > 0)
                                 .ToDictionary(x => x.Key, x => x.Value)
                    );
                }
            }
        }
        
        public void ValidateSearchTerm(string searchTerm)
        {
            var errors = new Dictionary<string, List<string>>();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                errors.Add("SearchTerm", new List<string> { "El término de búsqueda no puede estar vacío." });
            }
            else if (searchTerm.Trim().Length < 2)
            {
                errors.Add("SearchTerm", new List<string> { "Debe ingresar al menos 2 letras para buscar." });
            }

            if (errors.Count > 0)
            {
                throw new ZuliValidationException(errors);
            }
        }
    }
}