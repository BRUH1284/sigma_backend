namespace sigma_backend.Models
{
    public class UserProfile
    {
        public required string UserId { get; set; }

        public string? Bio { get; set; }
        public string? ProfilePictureFileName { get; set; }
        public virtual ICollection<ProfilePicture>? ProfilePictures { get; set; }
    }
}