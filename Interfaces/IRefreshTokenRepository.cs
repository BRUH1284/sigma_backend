using sigma_backend.Models;

namespace sigma_backend.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByIdAsync(int id);

        Task<List<RefreshToken>> GetByUserIdAsync(string userId);

        Task<RefreshToken?> GetByUserIdAndDeviceIdAsync(string userId, string deviceId);

        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);

        Task<RefreshToken?> DeleteAsync(int id);
        Task<RefreshToken> UpdateOrCreateAsync(RefreshToken refreshToken);
    }
}
