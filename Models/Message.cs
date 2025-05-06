namespace sigma_backend.Models
{
    public class Message
    {
        public required string Id { get; set; }
        public required string SenderUsername { get; set; }
        public required string ReceiverUsername { get; set; }
        public required string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}