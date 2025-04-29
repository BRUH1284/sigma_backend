using System.ComponentModel.DataAnnotations;
using sigma_backend.Enums;

namespace sigma_backend.DataTransferObjects.User
{
    public class UpdateUserProfileSettingsRequestDto
    {
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required]
        public required string Bio { get; set; }
        [Required]
        public required bool FriendsVisible { get; set; }
        [Required]
        public float Weight { get; set; }
        [Required]
        public float TargetWeight { get; set; }
        [Required]
        public float Height { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public UserActivityLevel ActivityLevel { get; set; }
        [Required]
        public UserClimate UserClimate { get; set; }
        [Required]
        public UserGoal Goal { get; set; }
    }
}