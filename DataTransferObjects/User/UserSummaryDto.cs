namespace sigma_backend.DataTransferObjects.User
{
    public class UserSummaryDto
    {
        public required string UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}