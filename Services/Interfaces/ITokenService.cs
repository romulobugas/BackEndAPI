namespace BackEndAPI.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateTemporaryToken(string username);
        bool ValidateTemporaryToken(string token);
    }
}
