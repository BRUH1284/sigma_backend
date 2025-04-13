namespace sigma_backend.DataTransferObjects.CustomFood
{
    public class CreateCustomFoodDto
    {
        public string Food { get; set; } = null!;
        public float CaloricValue { get; set; }
        public float Protein { get; set; }
        public float Fat { get; set; }
        public float Carbohydrates { get; set; }
    }
}
