using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Models;

namespace sigma_backend.Repository
{
    public class FriendshipRepository : RepositoryBase, IFriendshipRepository
    {
        public FriendshipRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Friendship> CreateAsync(string userId, string friendId)
        {
            // Create new friendships
            var friendshipA = new Friendship
            {
                UserId = userId,
                FriendId = friendId
            };

            var friendshipB = new Friendship
            {
                UserId = friendId,
                FriendId = userId
            };

            // Save new friendship
            await _context.Friendships.AddAsync(friendshipA);
            await _context.Friendships.AddAsync(friendshipB);
            await _context.SaveChangesAsync();
            return friendshipA;
        }
        public async Task<Friendship?> DeleteAsync(string userId, string friendId)
        {
            // Get friendship
            var friendshipA = await _context.Friendships.FindAsync(userId, friendId);
            var friendshipB = await _context.Friendships.FindAsync(friendId, userId);

            // Remove friendships
            if (friendshipA != null)
                _context.Remove(friendshipA);

            if (friendshipB != null)
                _context.Remove(friendshipB);

            await _context.SaveChangesAsync();

            return friendshipA;
        }
        public async Task<Friendship?> GetAsync(string userId, string friendId)
        {
            return await _context.Friendships.FindAsync(userId, friendId);
        }
        public async Task<List<User?>> GetFriends(string userId)
        {
            return await _context.Friendships
                .Where(f => f.UserId == userId)
                .Select(f => f.Friend)
                .ToListAsync();
        }
        public async Task<int> GetFriendCount(string userId)
        {
            return await _context.Friendships.CountAsync(f => f.UserId == userId);
        }
    }
}