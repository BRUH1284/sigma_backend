namespace sigma_backend.Models
{
    public class UserFollower
    {
        public required string FollowerId { get; set; }
        public required string FolloweeId { get; set; }

        public virtual User? Follower { get; set; }
        public virtual User? Followee { get; set; }

        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }
}