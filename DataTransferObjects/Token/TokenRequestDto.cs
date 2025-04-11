using System.ComponentModel.DataAnnotations;

namespace sigma_backend.DataTransferObjects.Token
{
    public class TokenRequestDto
    {
        [Required]
        public required string OldAccessToken { get; set; }
        [Required]
        public required string RefreshToken { get; set; }
        [Required]
        public required string DeviceId { get; set; }
    }
}