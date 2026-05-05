using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zuli_Buisiness.DTO;
using zuli_Data.Exceptions;

namespace zuli_Buisiness.Validation
{
    public class FlightSearchValidator
    {
        public void ValidateSearch(FlightSearchRequestDTO request)
        {
            var errors = new Dictionary<string, List<string>>();

            if (string.IsNullOrWhiteSpace(request.Origin) || request.Origin.Length != 3)
                AddError(errors, "Origin", "El origen debe tener exactamente 3 caracteres.");

            if (string.IsNullOrWhiteSpace(request.Destination) || request.Destination.Length != 3)
                AddError(errors, "Destination", "El destino debe tener exactamente 3 caracteres.");

            if (request.Origin?.ToUpper() == request.Destination?.ToUpper())
                AddError(errors, "Destination", "El origen y destino no pueden ser iguales.");

            if (request.Date.Date < DateTime.Now.Date)
                AddError(errors, "Date", "La fecha de búsqueda no puede ser en el pasado.");

            if (request.Seats <= 0)
                AddError(errors, "Seats", "Debe buscar al menos 1 asiento.");

            if (request.Page <= 0 || request.PageSize <= 0)
                AddError(errors, "Pagination", "Página y tamaño de página deben ser mayores a 0.");

            if (errors.Count > 0)
                throw new ZuliValidationException(errors);
        }

        private void AddError(Dictionary<string, List<string>> dict, string key, string message)
        {
            if (!dict.ContainsKey(key)) dict[key] = new List<string>();
            dict[key].Add(message);
        }
    }
}