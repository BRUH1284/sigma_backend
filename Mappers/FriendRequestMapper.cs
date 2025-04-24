using sigma_backend.DataTransferObjects.Friendship;
using sigma_backend.Models;

namespace sigma_backend.Mappers
{
    public static class FriendRequestMapper
    {
        public static FriendRequestDto ToFriendRequestDto(this FriendRequest friendRequest)
        {
            return new FriendRequestDto
            {
                SenderUsername = friendRequest.Sender!.UserName!,
                ReceiverUsername = friendRequest.Receiver!.UserName!,
                CreatedAt = friendRequest.RequestedAt
            };
        }
    }
}