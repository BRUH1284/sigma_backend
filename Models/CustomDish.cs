namespace sigma_backend.Models
{
    public class CustomDish
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual List<CustomDishIngredient> Ingredients { get; set; } = new();
    }
}
