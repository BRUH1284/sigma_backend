namespace sigma_backend.Models
{
    public class Friendship
    {
        public required string UserId { get; set; }
        public required string FriendId { get; set; }

        public virtual User? User { get; set; }
        public virtual User? Friend { get; set; }

        public DateTime FriendsSince { get; set; } = DateTime.UtcNow;
    }
}