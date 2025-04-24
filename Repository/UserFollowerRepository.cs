using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Models;

namespace sigma_backend.Repository
{
    public class UserFollowerRepository : RepositoryBase, IUserFollowerRepository
    {
        public UserFollowerRepository(ApplicationDbContext context) : base(context) { }

        public async Task<UserFollower> CreateAsync(string followerId, string followeeId)
        {
            // Create new follower
            var follower = new UserFollower
            {
                FollowerId = followerId,
                FolloweeId = followeeId
            };

            // Save new follower
            await _context.UserFollowers.AddAsync(follower);
            await _context.SaveChangesAsync();
            return follower;
        }

        public async Task<UserFollower?> DeleteAsync(string followerId, string followeeId)
        {
            // Get follower
            var follower = await _context.UserFollowers.FindAsync(followerId, followeeId);

            if (follower == null)
                return null;

            // Remove follower
            _context.Remove(follower);
            await _context.SaveChangesAsync();
            return follower;
        }

        public async Task<UserFollower?> GetAsync(string followerId, string followeeId)
        {
            return await _context.UserFollowers.FindAsync(followerId, followeeId);
        }

        public async Task<List<User?>> GetFollowers(string followeeId)
        {
            return await _context.UserFollowers
                .Where(f => f.FolloweeId == followeeId)
                .Select(f => f.Follower)
                .ToListAsync();
        }

        public async Task<List<User?>> GetFollowee(string followerId)
        {
            return await _context.UserFollowers
                .Where(f => f.FollowerId == followerId)
                .Select(f => f.Followee)
                .ToListAsync();
        }
        public async Task<int> GetFollowersCount(string followeeId)
        {
            return await _context.UserFollowers.CountAsync(f => f.FolloweeId == followeeId);
        }

        public async Task<int> GetFolloweeCount(string followerId)
        {
            return await _context.UserFollowers.CountAsync(f => f.FollowerId == followerId);
        }
    }
}