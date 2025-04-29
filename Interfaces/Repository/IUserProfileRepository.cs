using sigma_backend.DataTransferObjects.User;
using sigma_backend.Models;

namespace sigma_backend.Interfaces.Repository
{
    public interface IUserProfileRepository
    {
        Task<UserProfile> CreateAsync(UserProfile userProfile);
        Task<ProfilePicture> CreateProfilePicture(ProfilePicture profilePicture);
        Task<UserProfile?> UpdateAsync(string id, UpdateUserProfileSettingsRequestDto updateDto);
        Task<UserProfile?> UpdatePictureFileName(string id, string fileName);
        Task<ProfilePicture?> DeleteProfilePicture(string userId, string fileName);
    }
}