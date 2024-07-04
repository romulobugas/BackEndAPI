using BackEndAPI.Connections;
using BackEndAPI.Models;
using BackEndAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        public AuthController(ITokenService tokenService, AppDbContext dbContext)
        {
            _tokenService = tokenService;
            _dbContext = dbContext;
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            // Simulação de lógica de autenticação
            var user = _dbContext.AppUsers.FirstOrDefault(u => u.Name == username);
            if (user != null && VerifyPassword(password, user.Password))
            {
                var token = _tokenService.GenerateTemporaryToken(username);
                user.Token = token;
                _dbContext.SaveChanges(); // Salvar o token gerado no banco de dados

                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        [HttpGet("test")]
        public IActionResult Test(string token)
        {
            var user = _dbContext.AppUsers.FirstOrDefault(u => u.Token == token);
            if (user != null && _tokenService.ValidateTemporaryToken(token))
            {
                return Ok($"Hello, {user.Name}! You are authorized.");
            }

            return Unauthorized();
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
            return HashPassword(password) == hashedPassword;
        }
    }
}
