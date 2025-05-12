using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using sigma_backend.DataTransferObjects.Token;
using sigma_backend.Interfaces.Repository;


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

    [HttpPost("token")]
    public async Task<IActionResult> SaveToken([FromBody] PushTokenDto dto)
    {
        var username = User.Identity?.Name;
        if (username == null) return Unauthorized();

        await _userRepository.SaveExpoToken(username, dto.Token);
        return Ok();
    }
}
