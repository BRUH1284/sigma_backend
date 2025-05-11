namespace sigma_backend.Models
{
    public class UserActivityRecord : ActivityRecord
    {
        public required Guid ActivityId { get; set; }
        public virtual UserActivity? UserActivity { get; set; }
    }
}