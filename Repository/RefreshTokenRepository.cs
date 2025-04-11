using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.Interfaces;
using sigma_backend.Models;

namespace sigma_backend.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;
        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        // Get by ID
        public async Task<RefreshToken?> GetByIdAsync(int id)
        {
            return await _context.RefreshTokens.FindAsync(id);
        }
        // Get by UserId
        public async Task<List<RefreshToken>> GetByUserIdAsync(string userId)
        {
            return await _context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
        }
        // Get by UserId and DeviceId
        public async Task<RefreshToken?> GetByUserIdAndDeviceIdAsync(string userId, string deviceId)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId && rt.DeviceId == deviceId);
        }
        // Get by token hash
        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        }
        // Delete by ID
        public async Task<RefreshToken?> DeleteAsync(int id)
        {
            var refreshTokenModel = await _context.RefreshTokens.FindAsync(id);

            if (refreshTokenModel == null)
            {
                return null;
            }

            _context.RefreshTokens.Remove(refreshTokenModel);
            await _context.SaveChangesAsync();
            return refreshTokenModel;
        }
        // Update or create token
        public async Task<RefreshToken> UpdateOrCreateAsync(RefreshToken refreshToken)
        {
            // Check if the token already exists (based on UserId and DeviceId)
            var existingToken = await GetByUserIdAndDeviceIdAsync(refreshToken.UserId, refreshToken.DeviceId);

            if (existingToken != null)
            {
                // Token exists, update it
                existingToken.TokenHash = refreshToken.TokenHash;
                existingToken.ExpiresOn = refreshToken.ExpiresOn;
                _context.RefreshTokens.Update(existingToken);
            }
            else
            {
                // Token does not exist, create a new one
                await _context.RefreshTokens.AddAsync(refreshToken);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
            return refreshToken;
        }
    }
}