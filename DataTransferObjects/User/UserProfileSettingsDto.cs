using sigma_backend.Enums;

namespace sigma_backend.DataTransferObjects.User
{
    public class UserProfileSettingsDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Bio { get; set; }
        public required bool FriendsVisible { get; set; }
        public required float Weight { get; set; }
        public required float TargetWeight { get; set; }
        public required float Height { get; set; }
        public required Gender Gender { get; set; }
        public required UserActivityLevel ActivityLevel { get; set; }
        public required UserClimate UserClimate { get; set; }
        public required UserGoal Goal { get; set; }
    }
}