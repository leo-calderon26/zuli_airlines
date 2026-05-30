using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using zuli_Business.DTO;
using zuli_Data.Exceptions;

namespace zuli_Business.Validation
{
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
