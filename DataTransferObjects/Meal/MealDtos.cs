namespace sigma_backend.DataTransferObjects.Meal
{
    public class CreateMealDto
    {
        public int FoodId { get; set; }
        public double AmountInGrams { get; set; }
    }

    public class UpdateMealDto
    {
        public double AmountInGrams { get; set; }
    }
}
