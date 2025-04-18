using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Models;

namespace sigma_backend.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;
        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Post> CreateAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }
        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _context.Posts.FindAsync(id);
        }
        public async Task<Post?> DeleteAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
                return null;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<List<PostImage>> AddImagesAsync(List<PostImage> postImages)
        {
            if (postImages == null || postImages.Count == 0)
                return new List<PostImage>();

            // Add Post Images
            _context.PostImages.AddRange(postImages);
            await _context.SaveChangesAsync();

            return postImages;
        }

        public async Task<List<PostImage>> GetImagesForPostAsync(int postId)
        {
            return await _context.PostImages
                .Where(pi => pi.PostId == postId)
                .ToListAsync();
        }
    }
}