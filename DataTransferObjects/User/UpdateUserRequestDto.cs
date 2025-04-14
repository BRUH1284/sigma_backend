using System.ComponentModel.DataAnnotations;

namespace sigma_backend.DataTransferObjects.User
{
    public class UpdateUserRequestDto
    {
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
    }
}