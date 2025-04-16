namespace sigma_backend.Models
{
    public class UserProfile
    {
        public required string UserId { get; set; }
        public required virtual User User { get; set; }

        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}