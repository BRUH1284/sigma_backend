using System.ComponentModel.DataAnnotations;

namespace sigma_backend.DataTransferObjects.Activity
{
    public class UpdateActivityRequestDto
    {
        [Required]
        public required int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required int KcalPerHour { get; set; }
    }
}