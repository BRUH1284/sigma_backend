using sigma_backend.Models;

namespace sigma_backend.Interfaces.Repository
{
    public interface IPostRepository
    {
        Task<Post> CreateAsync(Post post);
        Task<Post?> GetByIdAsync(int id);
        Task<Post?> DeleteAsync(int id);
        Task<List<PostImage>> AddImagesAsync(List<PostImage> postImages);
        Task<List<PostImage>> GetImagesForPostAsync(int postId);
    }
}