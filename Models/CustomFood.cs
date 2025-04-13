using System.ComponentModel.DataAnnotations;

namespace sigma_backend.Models
{
    public class CustomFood
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public string Food { get; set; } = null!;

        [Required]
        public float CaloricValue { get; set; }

        public float Protein { get; set; }
        public float Fat { get; set; }
        public float Carbohydrates { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
