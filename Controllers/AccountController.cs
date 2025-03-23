using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sigma_backend.DataTransferObjects.Account;
using sigma_backend.Interfaces;
using sigma_backend.Models;

namespace sigma_backend.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ITokenService _tokenService;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };

                var createdUser = await userManager.CreateAsync(user, registerDto.Password);
                if (!createdUser.Succeeded)
                    return StatusCode(500, createdUser.Errors);
                // Add role
                var roleResult = await userManager.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                    return StatusCode(500, roleResult.Errors);
                // Retrieve role after adding
                var roles = await userManager.GetRolesAsync(user);

                return Ok(
                    new NewUserDto
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        Token = _tokenService.CreateToken(user, roles)
                    }
                );
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

            var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());
            if (user == null)
                return Unauthorized("Username not found and/or password is incorrect!");

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Username not found and/or password is incorrect!");

            // Retrieve user role
            var roles = await userManager.GetRolesAsync(user);
            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user, roles)
                }
            );
        }
    }
}