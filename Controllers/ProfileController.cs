using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sigma_backend.DataTransferObjects.User;
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
        private readonly ICurrentUserService _currentUserService;
        private readonly IPathService _pathService;
        private readonly IFileService _fileService;

        public ProfileController(
            UserManager<User> userManager,
            IUserRepository userRepo,
            IUserProfileRepository profileRepo,
            ICurrentUserService currentUserService,
            IPathService pathService,
            IFileService fileService)
        {
            _userManager = userManager;
            _userRepo = userRepo;
            _profileRepo = profileRepo;
            _currentUserService = currentUserService;
            _pathService = pathService;
            _fileService = fileService;
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            return Ok(user.ToUserProfileDto());
        }
        [HttpGet("{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfile(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return NotFound();

            return Ok(user.ToUserProfileDto());
        }
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserProfileRequestDto updateDto)
        {
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            user = await _userRepo.UpdateAsync(user.Id, updateDto.ToUpdateUserRequestDto());

            if (user?.Profile == null)
                return NotFound();

            var profile = await _profileRepo.UpdateAsync(user.Profile.UserId, updateDto);

            if (profile == null)
                return NotFound();


            return Ok(user.ToUserProfileDto());
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
            var folderPath = _pathService.GetProfilePictureFolderPath(user.UserName);

            // Save profile picture
            var fileName = await _fileService.SaveImageAsync(file, folderPath);

            // Get profile picture path
            var path = _pathService.GetProfilePicturePath(user.UserName, fileName);

            // If saving failed or updating DB failed, return server error
            if (path == null || await _profileRepo.UpdatePictureFileName(user.Profile.UserId, fileName) == null)
                return Problem("Server failed to save the profile picture.");

            // Build the public URL to access the uploaded image
            var url = _pathService.BuildPublicUrl(Request, path);

            return Ok(new { Url = url });
        }
        [HttpGet("{username}/picture")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadProfilePicture(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            var fileName = user?.Profile?.ProfilePictureFileName;

            if (fileName == null)
                return NotFound();

            var path = _pathService.GetProfilePicturePath(username, fileName);

            var contentType = _fileService.GetContentType(fileName);

            return path == null ? NotFound() : File(_fileService.GetFileStream(path), contentType, fileName);
        }
        [HttpGet("me/picture")]
        public async Task<IActionResult> DownloadMyProfilePicture()
        {
            var user = await _currentUserService.GetCurrentUserAsync(User);
            if (user == null || user.UserName == null)
                return Unauthorized();

            return await DownloadProfilePicture(user.UserName);
        }
        [HttpDelete("me/picture")]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            var user = await _currentUserService.GetCurrentUserAsync(User);
            if (user?.UserName == null || user.Profile == null)
                return Unauthorized();

            // Delete old profile picture
            if (user.Profile.ProfilePictureFileName != null)
            {
                var path = _pathService.GetProfilePicturePath(user.UserName, user.Profile.ProfilePictureFileName);

                if (path != null)
                    _fileService.DeleteFile(path);

                await _profileRepo.DeletePictureFileName(user.Profile.UserId);
            }
            return NoContent();
        }
    }
}