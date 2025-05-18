using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using sigma_backend.DataTransferObjects.Token;
using sigma_backend.Interfaces.Repository;

namespace sigma_backend.Controllers
{
    /// <summary>
    /// Handles push notification token registration.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public NotificationsController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Saves the Expo push token for the currently authenticated user.
        /// </summary>
        /// <param name="dto">DTO containing the Expo push token.</param>
        /// <returns>200 OK if successful, 401 Unauthorized otherwise.</returns>
        [HttpPost("token")]
        public async Task<IActionResult> SaveToken([FromBody] PushTokenDto dto)
        {
            var username = User.Identity?.Name;
            if (username == null) return Unauthorized();

            await _userRepository.SaveExpoToken(username, dto.Token);
            return Ok();
        }
    }
}
