using System.ComponentModel.DataAnnotations;

namespace sigma_backend.DataTransferObjects
{
    public class CreateActivityRequestDto
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required int KcalPerHour { get; set; }
    }
}