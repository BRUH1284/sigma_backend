using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sigma_backend.DataTransferObjects.User;
using sigma_backend.Extensions;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Interfaces.Service;
using sigma_backend.Mappers;
using sigma_backend.Models;

namespace sigma_backend.Controllers
{
    [Route("api/profiles")]
    [Authorize]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserProfileRepository _profileRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUserFollowerRepository _followerRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPathService _pathService;
        private readonly IFileService _fileService;

        public ProfileController(
            UserManager<User> userManager,
            IUserRepository userRepo,
            IUserFollowerRepository followerRepo,
            IUserProfileRepository profileRepo,
            ICurrentUserService currentUserService,
            IPathService pathService,
            IFileService fileService)
        {
            _userManager = userManager;
            _userRepo = userRepo;
            _profileRepo = profileRepo;
            _followerRepo = followerRepo;
            _currentUserService = currentUserService;
            _pathService = pathService;
            _fileService = fileService;
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            // Return user profile
            return Ok(user.ToUserProfileDto(user.GetProfilePictureUrl(Request, _pathService)));
        }
        [HttpGet("me/settings")]
        public async Task<IActionResult> GetMyProfileSettings()
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            // Return user profile
            return Ok(user.Profile!.ToUserProfileSettingsDto());
        }
        [HttpGet("{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfile(string username)
        {
            // Get user by username
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return NotFound();

            // Return user profile
            return Ok(user.ToUserProfileDto(user.GetProfilePictureUrl(Request, _pathService)));
        }
        [HttpPut("me/settings")]
        public async Task<IActionResult> UpdateMyProfileSettings([FromBody] UpdateUserProfileSettingsRequestDto updateDto)
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user?.Profile == null)
                return NotFound();

            // Update user profile in DB
            var profile = await _profileRepo.UpdateAsync(user.Profile.UserId, updateDto);

            if (profile == null)
                return NotFound();

            // Return user profile
            return Ok(user.Profile.ToUserProfileSettingsDto());
        }
        [HttpPost("me/picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            // Check if the file is provided
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Validate that the file is an image
            if (!_fileService.IsImage(file))
                return BadRequest("File is not an image");

            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);
            if (user?.UserName == null || user.Profile == null)
                return Unauthorized();

            // Get profile picture folder path
            var folderPath = _pathService.GetProfilePictureDirectoryPath(user.UserName);

            // Save profile picture
            var fileName = await _fileService.SaveImageAsync(file, folderPath);

            // Get profile picture path
            var path = _pathService.GetProfilePicturePath(user.UserName, fileName);

            // Generate profile picture
            var profilePicture = new ProfilePicture
            {
                UserId = user.Id,
                FileName = fileName
            };

            // If saving failed or updating DB failed, return server error
            if (path == null ||
                await _profileRepo.CreateProfilePicture(profilePicture) == null ||
                await _profileRepo.UpdatePictureFileName(user.Id, fileName) == null)
                return Problem("Server failed to save the profile picture.");

            // Build the public URL to access the uploaded image
            var url = _pathService.BuildPublicUrl(Request, path);
            return Created(url, new { Url = url });
        }
        [HttpGet("{username}/picture")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadProfilePicture(string username)
        {
            // Get user by username
            var user = await _userManager.FindByNameAsync(username);

            // Get user profile picture file name 
            var fileName = user?.Profile?.ProfilePictureFileName;

            if (fileName == null)
                return NotFound();

            // Get profile picture path
            var path = _pathService.GetProfilePicturePath(username, fileName);

            // Get picture type 
            var contentType = _fileService.GetContentType(fileName);

            // Return file if path is valid
            return path == null ? NotFound() : File(_fileService.GetFileStream(path), contentType, fileName);
        }
        [HttpGet("me/picture")]
        public async Task<IActionResult> DownloadMyProfilePicture()
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);
            if (user == null || user.UserName == null)
                return Unauthorized();

            // Get profile picture by username
            return await DownloadProfilePicture(user.UserName);
        }
        [HttpDelete("{username}/picture/{fileName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProfilePicture(string username, string fileName)
        {
            // Get user by username
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound();

            // Delete profile picture from DB
            var profilePicture = await _profileRepo.DeleteProfilePicture(user.Id, fileName);

            // Get profile picture file path
            var filePath = _pathService.GetProfilePicturePath(username, fileName);

            if (profilePicture == null || filePath == null)
                return NotFound();

            // Delete profile picture file
            _fileService.DeleteFile(filePath);

            return NoContent();
        }
        [HttpDelete("me/picture/{fileName}")]
        public async Task<IActionResult> DeleteProfilePicture(string fileName)
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);
            if (user?.UserName == null || user.Profile == null)
                return Unauthorized();

            // Delete profile picture by username
            return await DeleteProfilePicture(user.UserName, fileName);
        }
        [HttpPost("{username}/follow")]
        public async Task<IActionResult> FollowUser(string username)
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);
            if (user?.UserName == null)
                return Unauthorized();

            // Get target user by username
            var targetUser = await _userManager.FindByNameAsync(username);

            if (targetUser == null)
                return NotFound();

            if (targetUser == user)
                return BadRequest("User cannot follow themselves.");

            if (await _followerRepo.GetAsync(user.Id, targetUser.Id) != null)
                return BadRequest("User can only follow someone once.");

            // Save follow record in DB
            await _followerRepo.CreateAsync(user.Id, targetUser.Id);

            return Ok();
        }
        [HttpPost("{username}/unfollow")]
        public async Task<IActionResult> UnfollowUser(string username)
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);
            if (user?.UserName == null)
                return Unauthorized();

            // Get target user by username
            var targetUser = await _userManager.FindByNameAsync(username);

            if (targetUser == null)
                return NotFound();

            // Delete follow record from DB
            var follower = await _followerRepo.DeleteAsync(user.Id, targetUser.Id);

            if (follower == null)
                return NotFound();

            return NoContent();
        }
        [HttpGet("{username}/followers")]
        public async Task<IActionResult> GetFollowers(string username)
        {
            // Get user by username
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound();

            // Get user summary dto for each follower
            var followersSummaries = (await _followerRepo.GetFollowers(user.Id))
                .Select(f => f?.ToUserSummaryDto(user.GetProfilePictureUrl(Request, _pathService)))
                .ToList();

            return Ok(followersSummaries);
        }
        [HttpGet("me/followers")]
        public async Task<IActionResult> GetMyFollowers()
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);
            if (user?.UserName == null)
                return Unauthorized();

            return await GetFollowers(user.UserName);
        }
        // Get Followee
        [HttpGet("{username}/following")]
        public async Task<IActionResult> GetFollowings(string username)
        {
            // Get user by username
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound();

            // Get user summary dto for each followee
            var followeeSummaries = (await _followerRepo.GetFollowee(user.Id))
                .Select(f => f?.ToUserSummaryDto(user.GetProfilePictureUrl(Request, _pathService)))
                .ToList();

            return Ok(followeeSummaries);
        }
        [HttpGet("me/following")]
        public async Task<IActionResult> GetMyFollowings()
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);
            if (user?.UserName == null)
                return Unauthorized();

            return await GetFollowings(user.UserName);
        }
    }
}