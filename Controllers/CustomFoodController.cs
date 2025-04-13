using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.DataTransferObjects.CustomFood;
using sigma_backend.Models;
using System.Security.Claims;

namespace sigma_backend.Controllers
{
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
