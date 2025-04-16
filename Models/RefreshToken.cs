namespace sigma_backend.Models
{
    public class RefreshToken
    {
        public required string UserId { get; set; }
        public required string DeviceId { get; set; }
        public required string TokenHash { get; set; }
        public required DateTime ExpiresOn { get; set; }

        public bool IsActive => ExpiresOn > DateTime.UtcNow;
    }
}