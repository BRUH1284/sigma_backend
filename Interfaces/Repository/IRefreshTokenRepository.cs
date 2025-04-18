using sigma_backend.Models;

namespace sigma_backend.Interfaces.Repository
{
    public interface IRefreshTokenRepository
    {
        Task<List<RefreshToken>> GetByUserIdAsync(string userId);
        Task<RefreshToken?> GetByUserIdAndDeviceIdAsync(string userId, string deviceId);
        Task<RefreshToken?> DeleteAsync(string userId, string deviceId);
        Task<RefreshToken> UpdateOrCreateAsync(RefreshToken refreshToken);
    }
}
