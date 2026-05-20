using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using zuli_Buisiness.DTO;
using zuli_Business.Validation;
using zuli_Data.Exceptions;

namespace zuli_Business.Validation
{
    // Esto son tag pasa saber donde esta el error
    public static class AuthorizationAtributes
    {
        public const string AIRLINENAME = "AirlineName";
    }
    public class AuthorizationValidator
    {
        public void ValidateAuthorizationInfo(AuthorizationDTO user)
        {
            var errorInfo = new Dictionary<string, List<string>>()
            {
                {AuthorizationAtributes.AIRLINENAME, new List<string>() }
            };

            var IsEmptyInfo = false;
            //Validar que el código de destino solo pueda traer letras 
            if (!MiscValidator.ContainsChars(user.airlineName))
            {
                errorInfo[AuthorizationAtributes.AIRLINENAME].Add("El nombre de la aerolínea no puede tener números ni caractes especiales");
                IsEmptyInfo = true;
            }
            if (IsEmptyInfo)
            {
                throw new ZuliValidationException(errorInfo.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value));
            }
            else
            {
                if (!((user.airlineName == "zuli") || (user.airlineName == "snoopy") ||
                    (user.airlineName == "air") || (user.airlineName == "mushu")))
                {
                    throw new ZuliUnauthorizedException($"El nombre de la aerolínea ingresada no es permitida {user.airlineName}");
                }
            }
        }
    }
}
