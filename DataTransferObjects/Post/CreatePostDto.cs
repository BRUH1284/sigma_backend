using System.ComponentModel.DataAnnotations;

namespace sigma_backend.DataTransferObjects.Post
{
    public class CreatePostDto
    {
        [Required]
        public required string Content { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}