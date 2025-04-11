using System.ComponentModel.DataAnnotations;

namespace sigma_backend.DataTransferObjects.Authentication
{
    public class LoginDto
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        public required string DeviceId { get; set; }
    }
}