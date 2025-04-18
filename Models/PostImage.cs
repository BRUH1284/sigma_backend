namespace sigma_backend.Models
{
    public class PostImage
    {
        public required int PostId { get; set; }
        public required string FileName { get; set; }
        public virtual Post? Post { get; set; }
    }
}