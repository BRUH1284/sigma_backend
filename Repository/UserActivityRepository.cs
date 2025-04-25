using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.DataTransferObjects.Activity;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Models;

namespace sigma_backend.Repository
{
    public class UserActivityRepository : RepositoryBase, IUserActivityRepository
    {
        public UserActivityRepository(ApplicationDbContext context) : base(context) { }

        public async Task<string> ComputeUserDataChecksum(string userId)
        {
            // Fetch the required user activities
            var activities = await _context.UserActivities
                .Where(ua => ua.UserId == userId)
                .Select(ua => new
                {
                    ua.MajorHeading,
                    ua.MetValue,
                    ua.Description,
                    ua.UpdatedAt
                })
                .ToListAsync();

            // Serialize the data and compute the SHA256 hash
            var json = JsonSerializer.Serialize(activities);
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));

            // Return the hash as a hexadecimal string
            return Convert.ToBase64String(hash);
        }

        public async Task<UserActivity> CreateAsync(UserActivity userActivityModel)
        {
            await _context.UserActivities.AddAsync(userActivityModel);
            await _context.SaveChangesAsync();
            return userActivityModel;
        }

        public async Task<UserActivity?> DeleteAsync(Guid id)
        {
            var userActivity = await _context.UserActivities.FindAsync(id);

            if (userActivity == null)
                return null;

            _context.UserActivities.Remove(userActivity);

            await _context.SaveChangesAsync();
            return userActivity;
        }

        public async Task<List<UserActivity>> GetAllByUserIdAsync(string userId)
        {
            return await _context.UserActivities.Where(ua => ua.UserId == userId).ToListAsync();
        }

        public async Task<UserActivity?> GetByIdAsync(Guid id)
        {
            return await _context.UserActivities.FindAsync(id);
        }

        public async Task<UserActivity?> UpdateAsync(Guid id, UpdateActivityRequestDto activityDto)
        {
            var existingUserActivity = await _context.UserActivities.FindAsync(id);

            if (existingUserActivity == null)
                return null;

            existingUserActivity.MajorHeading = activityDto.MajorHeading;
            existingUserActivity.MetValue = activityDto.MetValue;
            existingUserActivity.Description = activityDto.Description;
            existingUserActivity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingUserActivity;
        }
    }
}