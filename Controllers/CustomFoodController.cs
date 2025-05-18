using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.DataTransferObjects.CustomFood;
using sigma_backend.Models;
using System.Security.Claims;

namespace sigma_backend.Controllers
{

    /// <summary>
    /// API for managing user-defined custom food entries.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomFoodController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomFoodController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new custom food entry for the current user.
        /// </summary>
        /// <param name="dto">Custom food data including nutrition values.</param>
        /// <returns>The created custom food entry.</returns>
        /// <response code="200">Returns the newly created food entry.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost]
        public async Task<IActionResult> AddCustomFood([FromBody] CreateCustomFoodDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var food = new CustomFood
            {
                UserId = userId,
                Food = dto.Food,
                CaloricValue = dto.CaloricValue,
                Protein = dto.Protein,
                Fat = dto.Fat,
                Carbohydrates = dto.Carbohydrates
            };

            _context.CustomFoods.Add(food);
            await _context.SaveChangesAsync();

            return Ok(food);
        }

        // (дополнительно позже: GET /api/custom-food, DELETE, UPDATE)
    }
}
