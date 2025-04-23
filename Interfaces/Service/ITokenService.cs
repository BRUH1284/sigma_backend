using System.Security.Claims;
using sigma_backend.Models;

namespace sigma_backend.Interfaces.Service
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, IList<string> roles, DateTime expiresOn);
        (RefreshToken, string) GenerateRefreshToken(string userId, string deviceId, DateTime expiresOn);
        ClaimsPrincipal? GetPrincipalFromToken(string accessToken);
        bool VerifyRefreshToken(string refreshToken, string tokenHash);
    }
}