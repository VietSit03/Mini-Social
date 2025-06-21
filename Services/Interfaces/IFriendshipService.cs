using MiniSocialAPI.Models.DTOs;

namespace MiniSocialAPI.Services.Interfaces
{
    public interface IFriendshipService
    {
        Task<IEnumerable<UserDto>> GetFriendsAsync(Guid userId);
        Task<IEnumerable<FriendRequestDto>> GetPendingRequestsAsync(Guid userId, string direction);
        Task<bool> SendFriendRequestAsync(Guid senderId, Guid receiverId);
        Task AcceptFriendRequestAsync(Guid senderId, Guid receiverId);
        Task RejectFriendRequestAsync(Guid senderId, Guid receiverId);
        Task RemoveFriendAsync(Guid userId, Guid friendId);
    }
}
