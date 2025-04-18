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
    [Route("api/users")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        private readonly IUserRepository _userRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPathService _pathService;
        private readonly IFileService _fileService;

        public UserController(
            UserManager<User> userManager,
            IUserRepository userRepo,
            ICurrentUserService currentUserService,
            IFileService fileService,
            IPathService pathService)
        {
            _userManager = userManager;
            _userRepo = userRepo;
            _currentUserService = currentUserService;
            _fileService = fileService;
            _pathService = pathService;
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetMySummary()
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user?.UserName == null)
                return Unauthorized();

            // Return user summary
            return Ok(user.ToUserSummaryDto(user.GetProfilePictureUrl(Request, _pathService)));
        }
        [HttpPut("me")]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequestDto updateDto)
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            // Update DB
            user = await _userRepo.UpdateAsync(user.Id, updateDto);

            // Return user summary
            return Ok(user!.ToUserSummaryDto(user!.GetProfilePictureUrl(Request, _pathService)));
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be empty.");

            var users = await _userRepo.SearchByUsernameAsync(query);

            var result = users.Select(u => u.ToUserSummaryDto(u.GetProfilePictureUrl(Request, _pathService)));

            return Ok(result);
        }
        [HttpGet("{username}")]
        public async Task<IActionResult> GetSummary(string username)
        {
            // Get user by username
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound();

            // Return user summary
            return Ok(user.ToUserSummaryDto(user.GetProfilePictureUrl(Request, _pathService)));
        }
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMe()
        {
            // Get the current logged-in user
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user?.UserName == null)
                return Unauthorized();

            // Delete user by username
            return await DeleteUser(user.UserName);
        }
        [HttpDelete("{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            // Get user by username
            var user = await _userManager.FindByNameAsync(username);
            if (user?.UserName == null)
                return NotFound();

            // delete user directory
            var userDirectory = _pathService.GetUserUploadsDirectoryPath(user.UserName);
            if (userDirectory != null)
                _fileService.DeleteDirectory(userDirectory);


            // delete user from DB
            await _userManager.DeleteAsync(user);

            return NoContent();
        }
    }
}