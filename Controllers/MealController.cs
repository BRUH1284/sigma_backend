using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.DataTransferObjects.Meal;
using sigma_backend.Models;
using System.Security.Claims;

namespace sigma_backend.Controllers
{

    /// <summary>
    /// Handles operations related to meals tracking.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MealsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MealsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // POST /api/meals
        /// <summary>
        /// Creates a new meal entry for the authenticated user.
        /// </summary>
        /// <param name="dto">Meal details including food ID and amount.</param>
        /// <returns>The created meal record.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateMeal([FromBody] CreateMealDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token");


            var meal = new Meal
            {
                UserId = userId,
                FoodId = dto.FoodId,
                AmountInGrams = dto.AmountInGrams,
                Date = DateTime.UtcNow
            };

            _context.Meals.Add(meal);
            await _context.SaveChangesAsync();

            // Загрузим связанные данные
            await _context.Entry(meal).Reference(m => m.Food).LoadAsync();
            await _context.Entry(meal).Reference(m => m.User).LoadAsync();

            return Ok(meal);
        }

        // GET /api/meals/{userId}
        /// <summary>
        /// Gets all meal records for a specific user.
        /// </summary>
        /// <param name="userId">User ID to retrieve meals for.</param>
        /// <returns>List of meals.</returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMeals(string userId)
        {
            var meals = await _context.Meals
                .Include(m => m.Food)
                .Where(m => m.UserId == userId)
                .ToListAsync();

            return Ok(meals);
        }

        // PUT /api/meals/{id}
        /// <summary>
        /// Updates an existing meal entry.
        /// </summary>
        /// <param name="id">Meal ID.</param>
        /// <param name="dto">New values for the meal.</param>
        /// <returns>Updated meal object.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMeal(int id, [FromBody] UpdateMealDto dto)
        {
            var meal = await _context.Meals.FindAsync(id);
            if (meal == null) return NotFound();

            meal.AmountInGrams = dto.AmountInGrams;
            await _context.SaveChangesAsync();

            return Ok(meal);
        }

        // DELETE /api/meals/{id}
        /// <summary>
        /// Deletes a meal entry by ID.
        /// </summary>
        /// <param name="id">Meal ID to delete.</param>
        /// <returns>No content if deleted.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            var meal = await _context.Meals.FindAsync(id);
            if (meal == null) return NotFound();

            _context.Meals.Remove(meal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST /api/meals/{id}/upload
        /// <summary>
        /// Uploads a file (e.g. image or receipt) to a meal.
        /// </summary>
        /// <param name="id">Meal ID.</param>
        /// <param name="file">File to upload.</param>
        /// <returns>Path to the uploaded file.</returns>
        [HttpPost("{id}/upload")]
        public async Task<IActionResult> UploadFile(int id, IFormFile file)
        {
            var meal = await _context.Meals.FindAsync(id);
            if (meal == null) return NotFound();

            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var uploadsPath = Path.Combine(_env.WebRootPath ?? "Uploads", "meal-files");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var mealFile = new MealFile
            {
                MealId = meal.Id,
                FilePath = fullPath,
                FileType = file.ContentType,
                UploadedAt = DateTime.UtcNow
            };

            _context.MealFiles.Add(mealFile);
            await _context.SaveChangesAsync();

            return Ok(new { path = fileName });
        }

        // GET /api/meals/{id}/file
        /// <summary>
        /// Retrieves the file associated with a meal.
        /// </summary>
        /// <param name="id">Meal ID.</param>
        /// <returns>Binary file content.</returns>
        [HttpGet("{id}/file")]
        public async Task<IActionResult> GetFile(int id)
        {
            var file = await _context.MealFiles.FirstOrDefaultAsync(f => f.MealId == id);
            if (file == null || !System.IO.File.Exists(file.FilePath))
                return NotFound();

            var content = await System.IO.File.ReadAllBytesAsync(file.FilePath);
            return File(content, file.FileType, Path.GetFileName(file.FilePath));
        }

        // GET /api/Meals/summary
        /// <summary>
        /// Retrieves daily nutritional summary for a specific user and date.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="date">Date to summarize.</param>
        /// <returns>Nutritional summary for the day.</returns>
        [HttpGet("summary/{userId}")]
        public async Task<IActionResult> GetMealSummary(string userId, [FromQuery] DateTime date)
        {
            var utcDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            var meals = await _context.Meals
                .Include(m => m.Food)
                .Where(m => m.UserId == userId && m.Date.Date == utcDate.Date)
                .ToListAsync();

            if (meals == null || meals.Count == 0)
                return Ok(new MealSummaryDto());

            var summary = new MealSummaryDto
            {
                TotalCalories = meals.Sum(m => m.Food.CaloricValue * (float)(m.AmountInGrams / 100.0)),
                TotalProtein = meals.Sum(m => m.Food.Protein * (float)(m.AmountInGrams / 100.0)),
                TotalCarbohydrates = meals.Sum(m => m.Food.Carbohydrates * (float)(m.AmountInGrams / 100.0)),
                TotalFat = meals.Sum(m => m.Food.Fat * (float)(m.AmountInGrams / 100.0))
            };

            return Ok(summary);
        }
    }
}
