using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using sigma_backend.DataTransferObjects.User;
using sigma_backend.Interfaces;
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
        private string profilePictureFolderPath;

        public ProfileController(UserManager<User> userManager, IUserRepository userRepo, IUserProfileRepository profileRepo, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _userRepo = userRepo;
            _profileRepo = profileRepo;
            _currentUserService = currentUserService;
            profilePictureFolderPath = Path.Combine("wwwroot", "uploads", "profile-pictures");
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

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
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var user = await _currentUserService.GetCurrentUserAsync(User);
            if (user == null || user.Profile == null)
                return Unauthorized();

            // Create uploads folder if not exists
            Directory.CreateDirectory(profilePictureFolderPath);

            // Delete old profile picture
            if (user.Profile.ProfilePictureUrl != null)
            {
                var oldFilePath = Path.Combine(profilePictureFolderPath, user.Profile.ProfilePictureUrl);
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }

            // Generate unique file name
            var fileExt = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExt}";
            var filePath = Path.Combine(profilePictureFolderPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update DB
            await _profileRepo.UpdatePictureUrlAsync(user.Profile.UserId, fileName);

            return Ok(new { Url = user.Profile.ProfilePictureUrl });
        }
        [HttpGet("{username}/picture")]
        public async Task<IActionResult> DownloadProfilePicture(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound();

            var fileName = user.Profile?.ProfilePictureUrl;
            var filePath = Path.Combine(profilePictureFolderPath, fileName ?? "");

            if (fileName.IsNullOrEmpty() || !System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var contentType = "application/octet-stream";
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, contentType, fileName);
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
            if (user == null || user.Profile == null)
                return Unauthorized();

            // Delete old profile picture
            if (user.Profile.ProfilePictureUrl != null)
            {
                var oldFilePath = Path.Combine(profilePictureFolderPath, user.Profile.ProfilePictureUrl);
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }
            return NoContent();
        }
    }
}