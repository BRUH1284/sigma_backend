namespace sigma_backend.DataTransferObjects.User
{
    public class UserProfileDto
    {
        public string? UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public required bool FriendsVisible { get; set; }
        public int? FriendCount { get; set; }
        public required int followersCount { get; set; }
        public required int followeeCount { get; set; }
    }
}