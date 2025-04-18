using sigma_backend.DataTransferObjects.User;

namespace sigma_backend.DataTransferObjects.Post
{
    public class PostDto
    {
        public required int Id { get; set; }
        public required UserSummaryDto Author { get; set; }
        public required string Content { get; set; }
        public List<string>? ImageUrls { get; set; }
        public required DateTime createdAt { get; set; }
    }
}