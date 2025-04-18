using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.DataTransferObjects.User;
using sigma_backend.Interfaces.Repository;
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

        public async Task<ProfilePicture> CreateProfilePicture(ProfilePicture profilePicture)
        {
            await _context.ProfilePictures.AddAsync(profilePicture);
            await _context.SaveChangesAsync();
            return profilePicture;
        }

        public async Task<UserProfile?> UpdateAsync(string id, UpdateUserProfileRequestDto updateDto)
        {
            var existingProfile = await _context.UserProfiles.FindAsync(id);

            if (existingProfile == null)
                return null;

            existingProfile.Bio = updateDto.Bio;

            await _context.SaveChangesAsync();

            return existingProfile;
        }

        public async Task<UserProfile?> UpdatePictureFileName(string id, string fileName)
        {
            var userProfile = await _context.UserProfiles.FindAsync(id);

            if (userProfile == null)
                return null;

            userProfile.ProfilePictureFileName = fileName;

            await _context.SaveChangesAsync();

            return userProfile;
        }

        public async Task<ProfilePicture?> DeleteProfilePicture(string userId, string fileName)
        {
            var userProfile = await _context.UserProfiles.FindAsync(userId);
            var profilePicture = await _context.ProfilePictures.FindAsync(userId, fileName);

            if (userProfile == null || profilePicture == null)
                return null;

            // Remove the selected picture
            _context.ProfilePictures.Remove(profilePicture);

            await _context.SaveChangesAsync();
            if (userProfile.ProfilePictureFileName == profilePicture.FileName)
            {
                // Query remaining profile pictures for the user
                var remainingPictures = await _context.ProfilePictures
                    .Where(pp => pp.UserId == userId)
                    .OrderByDescending(pp => pp.FileName)
                    .ToListAsync();

                var lastPicture = remainingPictures.FirstOrDefault();

                userProfile.ProfilePictureFileName = lastPicture?.FileName;

                await _context.SaveChangesAsync();
            }

            return profilePicture;
        }
    }


}