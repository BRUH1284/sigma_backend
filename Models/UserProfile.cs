using sigma_backend.Enums;

namespace sigma_backend.Models
{
    public class UserProfile
    {
        public required string UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Bio { get; set; } = "";
        public bool FriendsVisible { get; set; } = true;
        public string? ProfilePictureFileName { get; set; }
        public virtual ICollection<ProfilePicture>? ProfilePictures { get; set; }
        public float Age { get; set; } = 0;
        public float Weight { get; set; } = 0;
        public float TargetWeight { get; set; } = 0;
        public float Height { get; set; } = 0;
        public Gender Gender { get; set; } = 0;
        public UserActivityLevel ActivityLevel { get; set; } = 0;
        public UserClimate UserClimate { get; set; } = 0;
        public UserGoal Goal { get; set; } = 0;
    }
}