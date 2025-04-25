using sigma_backend.Enums;

namespace sigma_backend.Models
{
    public class DataVersion
    {
        public required DataResource DataResource { get; set; }
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }
}