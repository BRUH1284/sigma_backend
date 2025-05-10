using sigma_backend.Enums;

namespace sigma_backend.Models
{
    public class ActivityRecord
    {
        public Guid Id { get; set; }
        public required string UserId { get; set; }
        public virtual User? User { get; set; }

        public required float Duration { get; set; }
        public required float Kcal { get; set; }
        public required DateTime Time { get; set; }
        public required DateTime LastModified { get; set; } = DateTime.UtcNow;

        public required RecordType Type { get; set; }  // Discriminator column
    }
}