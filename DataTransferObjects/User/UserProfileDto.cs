namespace sigma_backend.DataTransferObjects.User
{
    public class UserProfileDto
    {
        public string? UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Bio { get; set; }
    }
}