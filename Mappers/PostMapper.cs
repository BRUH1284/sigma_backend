using sigma_backend.DataTransferObjects.Post;
using sigma_backend.DataTransferObjects.User;
using sigma_backend.Models;

namespace sigma_backend.Mappers
{
    public static class PostMapper
    {
        public static PostDto ToPostDto(this Post post, UserSummaryDto author, List<string>? imageUrls = null)
        {
            return new PostDto
            {
                Id = post.Id,
                Author = author,
                Content = post.Content,
                ImageUrls = imageUrls,
                createdAt = post.CreatedAt
            };
        }
    }
}