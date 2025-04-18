namespace sigma_backend.Models
{
    public class Post
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public virtual User? User { get; set; }

        public required string Content { get; set; }
        public virtual ICollection<PostImage>? Images { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}