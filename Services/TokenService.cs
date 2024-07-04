using BackEndAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IDictionary<string, DateTime> _temporaryTokens = new Dictionary<string, DateTime>();

        public string GenerateTemporaryToken(string username)
        {
            var token = Guid.NewGuid().ToString();
            _temporaryTokens[token] = DateTime.UtcNow.AddMinutes(15); // Token válido por 15 minutos

            return token;
        }

        public bool ValidateTemporaryToken(string token)
        {
            if (_temporaryTokens.TryGetValue(token, out var expiration))
            {
                if (expiration > DateTime.UtcNow)
                {
                    // Token válido
                    return true;
                }
                else
                {
                    // Token expirado
                    _temporaryTokens.Remove(token);
                    return false;
                }
            }

            return false; // Token não encontrado
        }
    }
}
