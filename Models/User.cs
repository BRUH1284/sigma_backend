using Microsoft.AspNetCore.Identity;

namespace sigma_backend.Models
{
    public class User : IdentityUser
    {
        [ProtectedPersonalData]
        public required string FirstName { get; set; }
        [ProtectedPersonalData]
        public required string LastName { get; set; }

        public virtual UserProfile? Profile { get; set; }
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<UserFollower> Followers { get; set; } = new List<UserFollower>();
        public virtual ICollection<UserFollower> Following { get; set; } = new List<UserFollower>();
        public virtual ICollection<FriendRequest> FriendRequests { get; set; } = new List<FriendRequest>();
        public virtual ICollection<FriendRequest> ReceivedFriendRequests { get; set; } = new List<FriendRequest>();
        public virtual ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();

        // public virtual ICollection<Comment> Comments { get; set; }
        // public virtual ICollection<Like> Likes { get; set; }
    }
}