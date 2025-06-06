using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using sigma_backend.DataTransferObjects.Post;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Interfaces.Service;
using sigma_backend.Models;
using sigma_backend.Mappers;
using sigma_backend.Extensions;
using Microsoft.AspNetCore.Identity;

namespace sigma_backend.Controllers
{
    /// <summary>
    /// Controller for handling user posts.
    /// </summary>
    [Route("api/posts")]
    [Authorize]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IPostRepository _postRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPathService _pathService;
        private readonly IFileService _fileService;

        public PostController(
            UserManager<User> userManager,
            IPostRepository postRepo,
            ICurrentUserService currentUserService,
            IPathService pathService,
            IFileService fileService)
        {
            _userManager = userManager;
            _postRepo = postRepo;
            _currentUserService = currentUserService;
            _pathService = pathService;
            _fileService = fileService;
        }

        /// <summary>
        /// Creates a new post with optional image uploads.
        /// </summary>
        /// <param name="createPostDto">DTO containing post content and image files.</param>
        /// <returns>Created post with metadata and image URLs.</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto createPostDto)
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);
            if (user?.UserName == null || user.Profile == null)
                return Unauthorized();

            var post = new Post
            {
                UserId = user.Id,
                Content = createPostDto.Content
            };

            await _postRepo.CreateAsync(post);

            var userSummaryDto = user.ToUserSummaryDto(user.GetProfilePictureUrl(Request, _pathService));

            // If post has no images return it
            if (createPostDto.Images == null || createPostDto.Images.IsNullOrEmpty())
                return Ok(post.ToPostDto(userSummaryDto));

            // Get Post images folder path
            var folderPath = _pathService.GetPostImagesDirectoryPath(user.UserName, post.Id);

            // Save images
            var images = new List<PostImage>();
            var imageUrls = new List<string>();
            foreach (IFormFile file in createPostDto.Images)
            {
                var fileName = await _fileService.SaveImageAsync(file, folderPath);

                images.Add(new PostImage
                {
                    PostId = post.Id,
                    FileName = fileName
                });

                var url = _pathService.GetPostImageUrl(Request, user.UserName, post.Id, fileName);

                if (url != null)
                    imageUrls.Add(url);
            }

            // Add images to DB
            await _postRepo.AddImagesAsync(images);

            return CreatedAtAction(
                nameof(GetPostById),
                new { id = post.Id },
                post.ToPostDto(userSummaryDto, imageUrls)
            );
        }
        
        /// <summary>
        /// Returns the posts of the currently authenticated user.
        /// </summary>
        /// <returns>List of posts.</returns>
        [HttpGet("/api/profiles/me/posts")]
        public async Task<IActionResult> GetMyPosts()
        {
            // Get current user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user?.UserName == null)
                return Unauthorized();

            // Return user posts
            return await GetUserPosts(user.UserName);
        }
        
        /// <summary>
        /// Returns posts of a specified user by username.
        /// </summary>
        /// <param name="username">Username of the profile owner.</param>
        /// <returns>List of posts for the specified user.</returns>
        [HttpGet("/api/profiles/{username}/posts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserPosts(string username)
        {
            // Get user by username
            var user = await _userManager.FindByNameAsync(username);

            if (user?.UserName == null)
                return NotFound();

            var postDtos = new List<PostDto>();

            // User has no posts
            if (user.Posts == null)
                return Ok(postDtos);

            // Get all user posts
            foreach (Post post in user.Posts)
                postDtos.Add(GetPostDto(post));

            return Ok(postDtos);
        }

        /// <summary>
        /// Retrieves a single post by ID.
        /// </summary>
        /// <param name="id">ID of the post to retrieve.</param>
        /// <returns>The specified post, if it exists.</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _postRepo.GetByIdAsync(id);

            if (post == null)
                return NotFound();

            return Ok(GetPostDto(post));
        }
        
        /// <summary>
        /// Deletes a post by its ID. Admins can delete any post, users can delete their own.
        /// </summary>
        /// <param name="id">ID of the post to delete.</param>
        /// <returns>No content if deleted, 401 or 404 otherwise.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostById(int id)
        {
            Post? post;

            if (!User.IsInRole("Admin")) // Allow Admin to delete any post
            {
                // Check post owner
                post = await _postRepo.GetByIdAsync(id);

                if (post == null)
                    return NotFound();

                if (post?.User?.UserName != null && post.User.UserName != User?.Identity?.Name)
                    return Unauthorized();
            }

            // Delete post
            post = await _postRepo.DeleteAsync(id);

            if (post == null)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Builds and returns a PostDto including image URLs.
        /// </summary>
        /// <param name="post">Post entity to convert.</param>
        /// <returns>A fully constructed PostDto.</returns>
        private PostDto GetPostDto(Post post)
        {
            if (post?.User?.UserName == null)
                throw new InvalidOperationException("UserName cannot be null when creating post DTO.");

            var imageUrls = new List<string>();

            if (post.Images != null)
            {
                // Get post images
                foreach (PostImage postImage in post.Images)
                {
                    var imageUrl = _pathService.GetPostImageUrl(Request, post.User.UserName, post.Id, postImage.FileName);
                    if (imageUrl != null)
                        imageUrls.Add(imageUrl);
                }
            }

            return post.ToPostDto(post.User.ToUserSummaryDto(post.User.GetProfilePictureUrl(Request, _pathService)), imageUrls);
        }
    }
}