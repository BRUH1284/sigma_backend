using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.DataTransferObjects.User;
using sigma_backend.Interfaces;
using sigma_backend.Models;

namespace sigma_backend.Repository
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ApplicationDbContext _context;
        public UserProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<UserProfile> CreateAsync(UserProfile userProfile)
        {
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
            return userProfile;
        }

        public async Task<UserProfile?> UpdateAsync(string id, UpdateUserProfileRequestDto updateDto)
        {
            var existingProfile = await _context.UserProfiles.FindAsync(id);

            if (existingProfile == null)
            {
                return null;
            }

            existingProfile.Bio = updateDto.Bio;

            await _context.SaveChangesAsync();

            return existingProfile;
        }

        public async Task<UserProfile?> UpdatePictureUrlAsync(string id, string pictureUrl)
        {
            var existingProfile = await _context.UserProfiles.FindAsync(id);

            if (existingProfile == null)
            {
                return null;
            }

            existingProfile.ProfilePictureUrl = pictureUrl;

            await _context.SaveChangesAsync();

            return existingProfile;
        }
    }


}