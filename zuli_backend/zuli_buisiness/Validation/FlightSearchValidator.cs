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