using Microsoft.AspNetCore.Mvc; // Importing ASP.NET Core MVC framework
using sigma_backend.Services; // Importing the service layer
using sigma_backend.DataTransferObjects; // Importing Data Transfer Objects (DTOs)

namespace sigma_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService; // Service instance for business logic

        public UserController(IUserService userService)
        {
            _userService = userService; // Injecting the service via constructor
        }

        // Hanles HTTP GET request to fetch a single user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // Handles HTTP PUT request to update an existing user
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserRequestDto userDto)
        {
            try
            {
                await _userService.UpdateUserAsync(id, userDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // Handles HTTP DELETE request to delete a product by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}