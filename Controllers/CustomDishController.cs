using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.DataTransferObjects.CustomDish;
using sigma_backend.Models;
using System.Security.Claims;

namespace sigma_backend.Controllers
{

    /// <summary>
    /// Controller for managing user-created custom dishes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomDishController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomDishController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Adds a new custom dish for the authenticated user.
        /// </summary>
        /// <param name="dto">Custom dish data including ingredients.</param>
        /// <returns>The created custom dish with resolved ingredient names.</returns>
        /// <response code="200">Dish successfully created.</response>
        /// <response code="401">Unauthorized if user is not authenticated.</response>
        [HttpPost]
        public async Task<IActionResult> AddCustomDish([FromBody] CreateCustomDishDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var dish = new CustomDish
            {
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                Ingredients = dto.Ingredients.Select(i => new CustomDishIngredient
                {
                    FoodId = i.FoodId,
                    AmountInGrams = i.AmountInGrams
                }).ToList()
            };

            _context.CustomDishes.Add(dish);
            await _context.SaveChangesAsync();

            // Загружаем названия продуктов (иначе не будет FoodName в ответе)
            var foodNames = await _context.FoodNutrition
                .Where(f => dish.Ingredients.Select(i => i.FoodId).Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => f.Food);

            var result = new CustomDishDto
            {
                Id = dish.Id,
                Name = dish.Name,
                CreatedAt = dish.CreatedAt,
                Ingredients = dish.Ingredients.Select(i => new CustomDishIngredientDto
                {
                    FoodId = i.FoodId,
                    FoodName = foodNames.ContainsKey(i.FoodId) ? foodNames[i.FoodId] : "Unknown",
                    AmountInGrams = i.AmountInGrams
                }).ToList()
            };

            return Ok(result);
        }
    }
}
