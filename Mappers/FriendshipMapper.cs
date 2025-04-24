using sigma_backend.DataTransferObjects.Friendship;
using sigma_backend.Models;

namespace sigma_backend.Mappers
{
    public static class FriendshipMapper
    {
        public static FriendshipDto ToFriendshipDto(this Friendship friendship)
        {
            return new FriendshipDto
            {
                FriendUsername = friendship.Friend!.UserName!,
                FriendsSince = friendship.FriendsSince
            };
        }
    }
}