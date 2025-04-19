using sigma_backend.DataTransferObjects.User;
using sigma_backend.Models;

namespace sigma_backend.Mappers
{
    public static class UserMappers
    {
        public static UserSummaryDto ToUserSummaryDto(this User user, string? profilePictureUrl)
        {
            return new UserSummaryDto
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = profilePictureUrl
            };
        }
        public static UserProfileDto ToUserProfileDto(this User user, string? profilePictureUrl)
        {
            return new UserProfileDto
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = profilePictureUrl,
                Bio = user.Profile?.Bio,
                followersCount = user.Followers.Count,
                followeeCount = user.Following.Count
            };
        }
        public static UpdateUserRequestDto ToUpdateUserRequestDto(this UpdateUserProfileRequestDto dto)
        {
            return new UpdateUserRequestDto
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };
        }
    }
}