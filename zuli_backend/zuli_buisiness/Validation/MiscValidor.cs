using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace zuli_Business.Validation
{
    public static class MiscValidor
    {
        /// <summary>
        /// Verifica si la cadena contiene solo letras, números, guiones o barras diagonales (/), sin otros caracteres especiales.
        /// </summary>
        /// <param name="input">Texto a validar.</param>
        /// <returns>
        /// <c>true</c> si tiene combinación de letras, números, guiones y barras únicamente; en caso contrario, <c>false</c>.
        /// </returns>
        public static bool ContainsNumbersAndChars(string input)
            => !string.IsNullOrWhiteSpace(input) &&
               Regex.IsMatch(input, @"^[A-Za-z\d/-]+$");
    }
}
