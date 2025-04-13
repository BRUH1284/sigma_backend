namespace sigma_backend.DataTransferObjects.CustomDish
{
    public class CreateCustomDishDto
    {
        public string Name { get; set; } = null!;
        public List<DishIngredientDto> Ingredients { get; set; } = new();
    }

    public class DishIngredientDto
    {
        public int FoodId { get; set; }
        public double AmountInGrams { get; set; }
    }
}
