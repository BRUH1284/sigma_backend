using sigma_backend.DataTransferObjects.User;
using sigma_backend.Models;

namespace sigma_backend.Mappers
{
    public static class UserMappers
    {
        public static UserSummaryDto ToUserSummaryDto(this User user)
        {
            return new UserSummaryDto
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.Profile?.ProfilePictureUrl
            };
        }
        public static UserProfileDto ToUserProfileDto(this User user)
        {
            return new UserProfileDto
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Profile?.Bio
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