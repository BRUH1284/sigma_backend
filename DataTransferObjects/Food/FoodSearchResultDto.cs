using sigma_backend.Models;

namespace sigma_backend.DataTransferObjects.Food
{
    public class FoodSearchResultDto
    {
        public int TotalCount { get; set; }
        // public List<FoodNutrition> Items { get; set; } = new();
        public List<FoodSearchItemDto> Items { get; set; } = new();
    }
}
