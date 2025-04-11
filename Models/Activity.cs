using System.ComponentModel.DataAnnotations;

namespace sigma_backend.Models
{
    public class Activity
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required int KcalPerHour { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now.ToUniversalTime();
    }
}