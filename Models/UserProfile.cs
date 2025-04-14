using System.ComponentModel.DataAnnotations;

namespace sigma_backend.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}