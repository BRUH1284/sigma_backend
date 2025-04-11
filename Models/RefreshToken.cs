using System.ComponentModel.DataAnnotations;

namespace sigma_backend.Models
{
    public class RefreshToken
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public required string UserId { get; set; }
        [Required]
        public required string DeviceId { get; set; }
        [Required]
        public required string TokenHash { get; set; }
        [Required]
        public required DateTime ExpiresOn { get; set; }

        public bool IsActive => ExpiresOn > DateTime.UtcNow;
    }
}