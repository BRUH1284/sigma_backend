namespace sigma_backend.DataTransferObjects.User
{
    public class UserSummaryDto
    {
        public string? UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}