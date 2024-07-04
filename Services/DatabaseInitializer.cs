using Microsoft.EntityFrameworkCore;
using BackEndAPI.Connections;
using BackEndAPI.Models;
using BackEndAPI.Services.Interfaces;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BackEndAPI.Services;

namespace BackEndAPI.Services
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly AppDbContext _context;

        public DatabaseInitializer(AppDbContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            _context.Database.EnsureCreated();

            var userApi = _context.AppUsers.SingleOrDefault(u => u.Name == "UserAPI");
            if (userApi == null)
            {
                var hashedPassword = HashPassword("teste@123");
                var token = GenerateToken();

                userApi = new AppUser
                {
                    Name = "UserAPI",
                    Password = hashedPassword,
                    UpdateDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    
                };
                user.Token = _tokenService.GenerateToken(user);

                _context.AppUsers.Add(userApi);
                _context.SaveChanges();
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

        private string GenerateToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}
