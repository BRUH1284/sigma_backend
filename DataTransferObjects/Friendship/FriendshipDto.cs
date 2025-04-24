namespace sigma_backend.DataTransferObjects.Friendship
{
    public class FriendshipDto
    {
        public required string FriendUsername { get; set; }
        public required DateTime FriendsSince { get; set; }
    }
}