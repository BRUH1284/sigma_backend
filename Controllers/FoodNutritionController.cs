using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.DataTransferObjects.Food;
using sigma_backend.Models;
using System.Security.Claims;


namespace sigma_backend.Controllers
{

    /// <summary>
    /// Provides search functionality for default and user-defined foods and dishes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FoodController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FoodController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/food/search?query=cheese&page=1&pageSize=10
        /// <summary>
        /// Searches for foods by name across standard, custom foods, and custom dishes.
        /// </summary>
        /// <param name="query">Search term to look up in food names.</param>
        /// <param name="page">Page number for pagination (default is 1).</param>
        /// <param name="pageSize">Number of results per page (default is 10, max 100).</param>
        /// <returns>A list of matching food items and total count.</returns>
        /// <response code="200">Returns matching food items.</response>
        /// <response code="400">If the query string is missing or invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet("search")]
        public async Task<IActionResult> SearchFood([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var skip = (page - 1) * pageSize;

            // 1. Стандартные продукты
            var defaultQuery = _context.FoodNutrition
                .Where(f => EF.Functions.ILike(f.Food, $"%{query}%"))
                .Select(f => new FoodSearchItemDto
                {
                    Id = f.Id,
                    Name = f.Food,
                    Type = "default"
                });

            // 2. Кастомные блюда (только текущего пользователя)
            var customDishesQuery = _context.CustomDishes
                .Where(d => d.UserId == userId && EF.Functions.ILike(d.Name, $"%{query}%"))
                .Select(d => new FoodSearchItemDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Type = "custom_dish"
                });

            // 3. Кастомные продукты (только текущего пользователя)
            var customFoodsQuery = _context.CustomFoods
                .Where(cf => cf.UserId == userId && EF.Functions.ILike(cf.Food, $"%{query}%"))
                .Select(cf => new FoodSearchItemDto
                {
                    Id = cf.Id,
                    Name = cf.Food,
                    Type = "custom_food"
                });

            // Объединяем всё
            var combinedQuery = defaultQuery
                .Union(customDishesQuery)
                .Union(customFoodsQuery);

            var totalCount = await combinedQuery.CountAsync();

            var items = await combinedQuery
                .OrderBy(i => i.Name)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new FoodSearchResultDto
            {
                TotalCount = totalCount,
                Items = items
            });
        }
    }
}
