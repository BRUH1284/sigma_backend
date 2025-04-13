using System.ComponentModel.DataAnnotations.Schema;

namespace sigma_backend.Models
{
    public class Meal
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int FoodId { get; set; }
        public double AmountInGrams { get; set; }
        public DateTime Date { get; set; }

        public User User { get; set; } = null!;

        [ForeignKey("FoodId")]
        public FoodNutrition Food { get; set; } = null!;
    }
}
