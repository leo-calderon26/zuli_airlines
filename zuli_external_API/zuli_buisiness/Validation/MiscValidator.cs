using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace zuli_Business.Validation
{
    public static class MiscValidator
    {
        /// Verifica si la cadena contiene solo letras, números, 
        /// guiones o barras diagonales (/), sin otros caracteres especiales.
        public static bool ContainsNumbersAndChars(string input)
            => !string.IsNullOrWhiteSpace(input) &&
               Regex.IsMatch(input, @"^[A-Za-z\d/-]+$");

        /// Verifica si la cadena contiene solo letras, sin números
        /// ni caracteres especiales.
        public static bool ContainsChars(string input)
            => !string.IsNullOrWhiteSpace(input) &&
               Regex.IsMatch(input, @"^[A-Za-z]+$");
    }
}
