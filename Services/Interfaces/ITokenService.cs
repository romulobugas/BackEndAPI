using BackEndAPI.Models;

namespace BackEndAPI.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(JwtPayload payload);
        bool ValidateToken(string token, out JwtPayload payload);
    }
}
