using sigma_backend.Models;

namespace sigma_backend.Interfaces.Repository
{
    public interface IFriendshipRepository
    {
        Task<Friendship> CreateAsync(string userId, string friendId);
        Task<Friendship?> DeleteAsync(string userId, string friendId);
        Task<Friendship?> GetAsync(string userId, string friendId);
        Task<List<User?>> GetFriends(string userId);
        Task<int> GetFriendCount(string userId);
    }
}