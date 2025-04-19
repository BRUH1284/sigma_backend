using sigma_backend.Models;

namespace sigma_backend.Interfaces.Repository
{
    public interface IUserFollowerRepository
    {
        Task<UserFollower> CreateAsync(string followerId, string followeeId);
        Task<UserFollower?> DeleteAsync(string followerId, string followeeId);
        Task<UserFollower?> GetAsync(string followerId, string followeeId);
        Task<List<User?>> GetFollowers(string followeeId);
        Task<List<User?>> GetFollowee(string followerId);
        Task<int> GetFollowersCount(string followeeId);
        Task<int> GetFolloweeCount(string followerId);
    }
}