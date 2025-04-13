namespace sigma_backend.DataTransferObjects.CustomDish
{
    public class CustomDishDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public List<CustomDishIngredientDto> Ingredients { get; set; } = new();
    }

    public class CustomDishIngredientDto
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; } = null!;
        public double AmountInGrams { get; set; }
    }
}
