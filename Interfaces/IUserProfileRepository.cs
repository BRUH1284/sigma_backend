using sigma_backend.DataTransferObjects.User;
using sigma_backend.Models;

namespace sigma_backend.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile> CreateAsync(UserProfile userProfile);
        Task<UserProfile?> UpdateAsync(string id, UpdateUserProfileRequestDto updateDto);
        Task<UserProfile?> UpdatePictureUrlAsync(string id, string pictureUrl);
    }
}