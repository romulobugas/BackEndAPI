using BackEndAPI.Models;
using BackEndAPI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BackEndAPI.Services
{
    using JwtPayloadModel = BackEndAPI.Models.JwtPayload;

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(JwtPayloadModel payload)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("Validator", payload.Validator),
                new Claim("User", payload.User),
                new Claim("Password", payload.Password),
                new Claim("String", payload.String)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token, out JwtPayloadModel payload)
        {
            payload = null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                payload = new JwtPayloadModel
                {
                    Validator = jwtToken.Claims.First(x => x.Type == "Validator").Value,
                    User = jwtToken.Claims.First(x => x.Type == "User").Value,
                    Password = jwtToken.Claims.First(x => x.Type == "Password").Value,
                    String = jwtToken.Claims.First(x => x.Type == "String").Value
                };

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
