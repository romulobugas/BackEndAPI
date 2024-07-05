using BackEndAPI.Connections;
using BackEndAPI.Models;
using BackEndAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _dbContext;
        private readonly string _validatorKey;

        public AuthController(ITokenService tokenService, AppDbContext dbContext, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _dbContext = dbContext;
            _validatorKey = configuration["Jwt:Key"];
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password, string validator)
        {
            if (validator != _validatorKey)
            {
                return Unauthorized("Invalid validator key.");
            }


            // Simulação de lógica de autenticação
            var user = _dbContext.AppUsers.FirstOrDefault(u => u.Name == username);
            if (user != null && VerifyPassword(HashPassword(password), user.Password))
            {
                var payload = new BackEndAPI.Models.JwtPayload
                {
                    Validator = validator,
                    User = username,
                    Password = user.Password,
                    String = string.Empty
                };

                var token = _tokenService.GenerateToken(payload);
                user.Token = token;
                _dbContext.SaveChanges(); // Salvar o token gerado no banco de dados

                return Ok(new { Token = token, Time = DateTime.Now.AddMinutes(15) });
            }

            return Unauthorized();
        }

        [HttpGet("test")]
        public IActionResult Test(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Verifica se o campo "Validator" do token corresponde à chave configurada
            var validatorClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Validator");
            if (validatorClaim == null || validatorClaim.Value != _validatorKey)
            {
                return Unauthorized("Invalid validator key.");
            }

            // Verifica se o campo "User" existe no token
            var userClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "User");
            if (userClaim == null)
            {
                return Unauthorized("User claim not found in token.");
            }
            var username = userClaim.Value;

            // Verifica se o campo "Password" existe no token
            var passwordClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Password");
            if (passwordClaim == null)
            {
                return Unauthorized("Password claim not found in token.");
            }
            var password = passwordClaim.Value;

            // Verifica o tempo de expiração ("exp") do token
            var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
            if (expClaim != null && long.TryParse(expClaim.Value, out var expValue))
            {
                var expDate = DateTimeOffset.FromUnixTimeSeconds(expValue).UtcDateTime;
                var expDateTimeLocal = expDate.ToLocalTime();

                if (expDateTimeLocal > DateTime.Now)
                {
                    var user = _dbContext.AppUsers.FirstOrDefault(u => u.Name == username);
                    if (user != null && VerifyPassword(password, user.Password))
                    {
                        return Ok(new { Message = $"Hello, {user.Name}! You are authorized.", Time = DateTime.UtcNow.AddMinutes(15) });
                    }
                    else
                    {
                        return Unauthorized("Invalid username or password.");
                    }
                }
                else
                {
                    return Unauthorized("Token expirou, favor, realizar o login novamente.");
                }
            }
            else
            {
                return Unauthorized("Expiration claim (exp) not found or invalid.");
            }
        }



        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return password == hashedPassword;
        }
    }
}
