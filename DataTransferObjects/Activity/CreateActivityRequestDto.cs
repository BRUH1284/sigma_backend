using System.ComponentModel.DataAnnotations;

namespace sigma_backend.DataTransferObjects.Activity
{
    public class CreateActivityRequestDto
    {
        [Required]
        public required int Code { get; set; }
        [Required]
        public required string MajorHeading { get; set; }
        [Required]
        public required float MetValue { get; set; }
        [Required]
        public required string Description { get; set; }
    }
}