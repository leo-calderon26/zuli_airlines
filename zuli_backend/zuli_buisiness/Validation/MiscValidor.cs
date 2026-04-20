using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace zuli_Buisiness.Validation
{
    public static class MiscValidor
    {
        public static bool ContainsNumber(string input) 
        {
            return !string.IsNullOrWhiteSpace(input) && Regex.IsMatch(input, @"\d");
        }
    }
}
