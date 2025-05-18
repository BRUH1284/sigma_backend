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
    /// <summary>
    /// Controller for managing users.
    /// </summary>
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

        /// <summary>
        /// Gets the summary information of the currently authenticated user.
        /// </summary>
        /// <returns>User summary data</returns>
        /// <response code="200">Returns the user summary</response>
        /// <response code="401">User is not authenticated</response>
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

        /// <summary>
        /// Searches users by partial username match.
        /// </summary>
        /// <param name="query">Username search query</param>
        /// <returns>List of matched user summaries</returns>
        /// <response code="200">Returns a list of users</response>
        /// <response code="400">If query is empty</response>
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be empty.");

            var users = await _userRepo.SearchByUsernameAsync(query);

            var result = users.Select(u => u.ToUserSummaryDto(u.GetProfilePictureUrl(Request, _pathService)));

            return Ok(result);
        }

        /// <summary>
        /// Gets the summary of a user by username.
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <returns>User summary</returns>
        /// <response code="200">User found</response>
        /// <response code="404">User not found</response>
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

        /// <summary>
        /// Deletes the currently authenticated user's account.
        /// </summary>
        /// <response code="204">User deleted</response>
        /// <response code="401">User not authenticated</response>
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

        /// <summary>
        /// Deletes a user account by username. Only accessible by Admins.
        /// </summary>
        /// <param name="username">Username of the user to delete</param>
        /// <response code="204">User deleted</response>
        /// <response code="404">User not found</response>
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