using sigma_backend.Models;

namespace sigma_backend.Interfaces.Repository
{
    public interface IFriendRequestRepository
    {
        Task<FriendRequest> CreateAsync(string senderId, string receiverId);
        Task<FriendRequest?> DeleteAsync(string senderId, string receiverId);
        Task<FriendRequest?> GetAsync(string senderId, string receiverId);
        Task<List<FriendRequest>> GetSendedFriendRequests(string senderId);
        Task<List<FriendRequest>> GetReceivedFriendRequests(string receiverId);
        Task<int> GetSendedFriendRequestCount(string senderId);
        Task<int> GetReceivedFriendRequestCount(string receiverId);
    }
}