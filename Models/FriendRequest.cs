namespace sigma_backend.Models
{
    public class FriendRequest
    {
        public required string SenderId { get; set; }
        public required string ReceiverId { get; set; }

        public virtual User? Sender { get; set; }
        public virtual User? Receiver { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}