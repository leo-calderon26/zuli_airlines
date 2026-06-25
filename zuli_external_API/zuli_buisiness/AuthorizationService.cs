using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Business.Validation;
using zuli_Data.Entities;
using zuli_Data.Exceptions;
using zuli_Repository.Interface;


namespace zuli_Business
{
    public class AuthorizationService : IAuthorizationService
    {
        public const int TOKENVALIDPERIOD = 60;
        private readonly string secretKey;
        private readonly string apiKey;
        private readonly AuthorizationValidator _validator;
        public AuthorizationService(IConfiguration config)
        {
            secretKey = config.GetSection("settings").GetSection("secretKey").Value.ToString();
            apiKey = config.GetSection("settings").GetSection("apiKey").Value.ToString();
            _validator = new AuthorizationValidator();
        }
        public async Task<AuthorizationResponseDTO> ValidateUser(AuthorizationDTO user)
        {
            _validator.ValidateAuthorizationInfo(user, apiKey);

            var keyBytes = Encoding.ASCII.GetBytes(secretKey);
            var claims = new ClaimsIdentity();

            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.airlineName));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddDays(TOKENVALIDPERIOD),
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
