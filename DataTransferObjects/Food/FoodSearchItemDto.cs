public class FoodSearchItemDto
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!; // "default", "custom_food", "custom_dish"
    public int? Id { get; set; } // can be null
}
