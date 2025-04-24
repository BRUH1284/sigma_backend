namespace sigma_backend.DataTransferObjects.Friendship
{
    public class FriendRequestDto
    {
        public required string SenderUsername { get; set; }
        public required string ReceiverUsername { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}