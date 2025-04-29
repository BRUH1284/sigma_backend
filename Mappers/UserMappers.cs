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
                UserName = user.UserName!,
                FirstName = user.Profile!.FirstName,
                LastName = user.Profile!.LastName,
                ProfilePictureUrl = profilePictureUrl
            };
        }
        public static UserProfileDto ToUserProfileDto(this User user, string? profilePictureUrl)
        {
            return new UserProfileDto
            {
                UserName = user.UserName,
                FirstName = user.Profile!.FirstName,
                LastName = user.Profile!.LastName,
                ProfilePictureUrl = profilePictureUrl,
                Bio = user.Profile?.Bio,
                FriendsVisible = user.Profile!.FriendsVisible,
                FriendCount = user.Profile!.FriendsVisible ? user.Friendships.Count : 0,
                followersCount = user.Followers.Count,
                followeeCount = user.Following.Count
            };
        }
        public static UserProfileSettingsDto ToUserProfileSettingsDto(this UserProfile userProfile)
        {
            return new UserProfileSettingsDto
            {
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                Bio = userProfile.Bio,
                FriendsVisible = userProfile.FriendsVisible,
                Weight = userProfile.Weight,
                TargetWeight = userProfile.TargetWeight,
                Height = userProfile.Height,
                Gender = userProfile.Gender,
                ActivityLevel = userProfile.ActivityLevel,
                UserClimate = userProfile.UserClimate,
                Goal = userProfile.Goal
            };
        }
    }
}