using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using zuli_Buisiness.DTO;
using zuli_Buisiness.Interface;
using zuli_Buisiness.Validation;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;


namespace zuli_Buisiness
{
    public class AuthorizationService : IAuthorizationService
    {
        // Inyeccion de dependencias
        private readonly string secretKey;
        private readonly AuthorizationValidator _validator;
        public AuthorizationService(IConfiguration config)
        {
            secretKey = config.GetSection("settings").GetSection("secretKey").ToString();
            _validator = new AuthorizationValidator();
        }
        public async Task<AuthorizationResponseDTO> ValidateUser(AuthorizationDTO user)
        {
            // aqui se tiene que llamar el 
            _validator.ValidateAuthorizationInfo(user);

            var keyBytes = Encoding.ASCII.GetBytes(secretKey);
            var claims = new ClaimsIdentity();

            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.airlineName));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            string tokenCreado = tokenHandler.WriteToken(tokenConfig);

            return new AuthorizationResponseDTO
            {
                StatusCode = 200,
                CreatedToken = tokenCreado,
            };
        }
    }
}
