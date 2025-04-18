namespace sigma_backend.Models
{
    public class CustomDishIngredient
    {
        public int Id { get; set; }

        public int CustomDishId { get; set; }
        public int FoodId { get; set; }
        public double AmountInGrams { get; set; }

        public virtual CustomDish CustomDish { get; set; } = null!;
        public virtual FoodNutrition Food { get; set; } = null!;
    }
}
