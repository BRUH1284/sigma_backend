namespace sigma_backend.Models
{
    public class UserProfile
    {
        public required string UserId { get; set; }

        public string? Bio { get; set; }
        public bool FriendsVisible { get; set; } = true;
        public string? ProfilePictureFileName { get; set; }
        public virtual ICollection<ProfilePicture>? ProfilePictures { get; set; }
        public float Weight { get; set; } // TODO: Update, dto?
        public float Height { get; set; }
    }
}