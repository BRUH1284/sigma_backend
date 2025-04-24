using Microsoft.EntityFrameworkCore;
using sigma_backend.Data;
using sigma_backend.Interfaces.Repository;
using sigma_backend.Models;

namespace sigma_backend.Repository
{
    public class FriendRequestRepository : RepositoryBase, IFriendRequestRepository
    {
        public FriendRequestRepository(ApplicationDbContext context) : base(context) { }

        public async Task<FriendRequest> CreateAsync(string senderId, string receiverId)
        {
            // Create new friend request
            var friendRequest = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId
            };

            // Save new friend request
            await _context.FriendRequests.AddAsync(friendRequest);
            await _context.SaveChangesAsync();
            return friendRequest;
        }
        public async Task<FriendRequest?> DeleteAsync(string senderId, string receiverId)
        {
            // Get friendship
            var friendRequest = await _context.FriendRequests.FindAsync(senderId, receiverId);

            if (friendRequest == null)
                return null;

            // Remove friendship
            _context.Remove(friendRequest);
            await _context.SaveChangesAsync();
            return friendRequest;
        }
        public async Task<FriendRequest?> GetAsync(string senderId, string receiverId)
        {
            return await _context.FriendRequests.FindAsync(senderId, receiverId);
        }
        public async Task<List<FriendRequest>> GetSendedFriendRequests(string senderId)
        {
            return await _context.FriendRequests
                .Where(r => r.SenderId == senderId)
                .ToListAsync();
        }
        public async Task<List<FriendRequest>> GetReceivedFriendRequests(string receiverId)
        {
            return await _context.FriendRequests
                .Where(r => r.ReceiverId == receiverId)
                .ToListAsync();
        }
        public async Task<int> GetSendedFriendRequestCount(string senderId)
        {
            return await _context.FriendRequests.CountAsync(r => r.SenderId == senderId);
        }
        public async Task<int> GetReceivedFriendRequestCount(string receiverId)
        {
            return await _context.FriendRequests.CountAsync(r => r.ReceiverId == receiverId);
        }
    }
}