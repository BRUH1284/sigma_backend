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

        public UserController(
            UserManager<User> userManager,
            IUserRepository userRepo,
            ICurrentUserService currentUserService,
            IPathService pathService)
        {
            _userManager = userManager;
            _userRepo = userRepo;
            _currentUserService = currentUserService;
            _pathService = pathService;
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetMySummary()
        {
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user?.UserName == null)
                return Unauthorized();

            string? ProfilePictureUrl = null;
            if (user.Profile?.ProfilePictureFileName != null)
                ProfilePictureUrl = _pathService.GetProfilePictureUrl(
                    Request,
                    user.UserName,
                    user.Profile.ProfilePictureFileName
                    );

            return Ok(user.ToUserSummaryDto(ProfilePictureUrl));
        }
        [HttpPut("me")]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequestDto updateDto)
        {
            var user = await _currentUserService.GetCurrentUserAsync(User);

            if (user == null)
                return Unauthorized();

            user = await _userRepo.UpdateAsync(user.Id, updateDto);

            return Ok(user?.ToUserSummaryDto(user.GetProfilePictureUrl(Request, _pathService)));
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
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound();

            return Ok(user.ToUserSummaryDto(user.GetProfilePictureUrl(Request, _pathService)));
        }
    }
}