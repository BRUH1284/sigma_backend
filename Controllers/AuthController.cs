using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sigma_backend.DataTransferObjects.Authentication;
using sigma_backend.DataTransferObjects.Token;
using sigma_backend.Interfaces;
using sigma_backend.Models;

namespace sigma_backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _refreshTokenRepo = refreshTokenRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Crete new user
                var user = new User
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };
                // Save new user
                var createdUser = await _userManager.CreateAsync(user, registerDto.Password);
                if (!createdUser.Succeeded)
                    return BadRequest(createdUser.Errors);

                // Add role
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                    return BadRequest(roleResult.Errors);
                // Retrieve roles
                var roles = await _userManager.GetRolesAsync(user);

                // Return tokens
                return Ok(await GetTokenDto(user, roles, registerDto.DeviceId));
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string unauthorizedMessage = "Username not found and/or password is incorrect!";

            // Check username
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());
            if (user == null)
                return Unauthorized(unauthorizedMessage);

            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return Unauthorized(unauthorizedMessage);

            // Retrieve user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Return tokens
            return Ok(await GetTokenDto(user, roles, loginDto.DeviceId));
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
        {
            // Find user in DB
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized("User not found");

            // Delete refresh token
            var token = await _refreshTokenRepo.GetByUserIdAndDeviceIdAsync(user.Id, dto.DeviceId);
            if (token != null)
            {
                await _refreshTokenRepo.DeleteAsync(token.Id);
            }

            return NoContent();
        }
        [HttpPost("logout-all")]
        [Authorize]
        public async Task<IActionResult> LogoutAll()
        {
            // Find user in DB
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized("User not found");

            // Delete all refresh tokens for this user
            var tokens = await _refreshTokenRepo.GetByUserIdAsync(user.Id);
            foreach (var token in tokens)
            {
                await _refreshTokenRepo.DeleteAsync(token.Id);
            }

            return NoContent();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest)
        {
            // Get username from access token
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenRequest.OldAccessToken);
            var userName = principal?.Identity?.Name;
            if (userName == null)
                return Unauthorized("Invalid token or username");

            // Find user in DB
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            if (user == null)
                return Unauthorized("User not found");

            // Validate token
            var storedToken = await _refreshTokenRepo.GetByUserIdAndDeviceIdAsync(user.Id, tokenRequest.DeviceId);
            if (storedToken == null || !_tokenService.VerifyRefreshToken(tokenRequest.RefreshToken, storedToken.TokenHash))
                return Unauthorized("Invalid refresh token");

            // Retrieve user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Return tokens
            return Ok(await GetTokenDto(user, roles, tokenRequest.DeviceId));
        }
        private async Task<TokenDto> GetTokenDto(User user, IList<string> roles, string deviceId)
        {
            // Set token expiration time
            var accessTokenExpirationTime = DateTime.Now.AddMinutes(30);
            var refreshTokenExpirationTime = DateTime.Now.AddDays(30).ToUniversalTime();

            // Generate access token
            var accessToken = _tokenService.GenerateAccessToken(user, roles, accessTokenExpirationTime);
            (var refreshTokenDto, var refreshTokenString) = _tokenService.GenerateRefreshToken(user.Id, deviceId, refreshTokenExpirationTime);

            // Save new or update existing token for specified user device 
            await _refreshTokenRepo.UpdateOrCreateAsync(refreshTokenDto);

            // Create token dto
            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString
            };
        }
        private async Task<User?> GetCurrentUserAsync()
        {
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return null;

            return await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        }
    }
}