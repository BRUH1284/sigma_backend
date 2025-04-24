using System.ComponentModel.DataAnnotations;

namespace sigma_backend.DataTransferObjects.User
{
    public class UpdateUserProfileRequestDto
    {
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required]
        public required string Bio { get; set; }
        [Required]
        public required bool FriendsVisible { get; set; }
    }
}