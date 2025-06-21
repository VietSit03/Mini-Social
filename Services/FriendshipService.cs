using Microsoft.EntityFrameworkCore;
using MiniSocialAPI.Models.DTOs;
using MiniSocialAPI.Models.Entities;
using MiniSocialAPI.Services.Interfaces;

namespace MiniSocialAPI.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly MiniSocialContext _context;

        public FriendshipService(MiniSocialContext context)
        {
            _context = context;
        }

        public async Task AcceptFriendRequestAsync(Guid senderId, Guid receiverId)
        {
            // Find the friend request sent from senderId to receiverId
            var request = await _context.FriendRequests
                .FirstOrDefaultAsync(x => x.SenderId == senderId && x.ReceiverId == receiverId);

            if (request == null)
                throw new InvalidOperationException("Friend request does not exist.");

            // Only pending requests can be rejected
            if (request.Status != "P")
            {
                throw new InvalidOperationException(request.Status switch
                {
                    "A" => "Friend request has already been accepted.",
                    "R" => "Friend request has already been rejected.",
                    _ => "Invalid friend request status."
                });
            }

            // TODO: Push noti

            // Update the status to Accepted
            request.Status = "A";

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserDto>> GetFriendsAsync(Guid userId)
        {
            // Find the accepted friend request between userId and friendId (regardless of direction)
            var query = _context.FriendRequests.Where(x =>
                x.Status == "A" && (x.SenderId == userId || x.ReceiverId == userId))
                .Include(x => x.Sender)
                .Include(x => x.Receiver);

            var data = await query.AsNoTracking()
                .Select(x => new UserDto
                {
                    Id = x.SenderId == userId ? x.Receiver.Id : x.Sender.Id,
                    FullName = x.SenderId == userId ? x.Receiver.FullName : x.Sender.FullName,
                    AvatarUrl = x.SenderId == userId ? x.Receiver.AvatarUrl : x.Sender.AvatarUrl
                })
                .ToListAsync();

            return data;
        }

        public async Task<IEnumerable<FriendRequestDto>> GetPendingRequestsAsync(Guid userId, string direction)
        {
            var query = direction.ToLower() switch
            {
                "sent" => _context.FriendRequests
                    .Where(x => x.SenderId == userId && x.Status == "P")
                    .Include(x => x.SenderId),

                "received" => _context.FriendRequests
                    .Where(x => x.ReceiverId == userId && x.Status == "P")
                    .Include(x => x.ReceiverId),

                "all" => _context.FriendRequests
                    .Where(x => (x.SenderId == userId || x.ReceiverId == userId) && x.Status == "P")
                    .Include(x => x.SenderId)
                    .Include(x => x.ReceiverId),

                _ => throw new ArgumentException("Invalid direction")
            };

            var data = await query
                .Select(x => new FriendRequestDto
                {
                    Id = x.Id,
                    SenderId = x.SenderId,
                    SenderName = x.Sender.FullName,
                    ReceiverId = x.ReceiverId,
                    ReceiverName = x.Receiver.FullName,
                    CreatedAt = x.CreatedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return data;
        }

        public async Task RejectFriendRequestAsync(Guid senderId, Guid receiverId)
        {
            // Find the friend request sent from senderId to receiverId
            var request = await _context.FriendRequests
                .FirstOrDefaultAsync(x => x.SenderId == senderId && x.ReceiverId == receiverId);

            if (request == null)
                throw new InvalidOperationException("Friend request does not exist.");

            // Only pending requests can be rejected
            if (request.Status != "P")
            {
                throw new InvalidOperationException(request.Status switch
                {
                    "A" => "Friend request has already been accepted.",
                    "R" => "Friend request has already been rejected.",
                    _ => "Invalid friend request status."
                });
            }

            // Update the status to Rejected
            request.Status = "R";

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFriendAsync(Guid userId, Guid friendId)
        {
            // Find the accepted friend request between userId and friendId (regardless of direction)
            var friendship = await _context.FriendRequests.FirstOrDefaultAsync(x =>
                x.Status == "A" &&
                ((x.SenderId == userId && x.ReceiverId == friendId) ||
                 (x.SenderId == friendId && x.ReceiverId == userId))
            );

            if (friendship == null)
                throw new InvalidOperationException("Friendship does not exist.");

            _context.FriendRequests.Remove(friendship);

            await _context.SaveChangesAsync();
        }


        public async Task<bool> SendFriendRequestAsync(Guid senderId, Guid receiverId)
        {
            // 1. Check receiver
            if (senderId == receiverId)
                throw new InvalidOperationException("Invalid receiver");

            // 2. Check friend request 2 user
            var existing = await _context.FriendRequests.FirstOrDefaultAsync(fr =>
                (fr.SenderId == senderId && fr.ReceiverId == receiverId) ||
                (fr.SenderId == receiverId && fr.ReceiverId == senderId)
            );

            if (existing != null)
            {
                // If request pending
                if (existing.Status == "P")
                {
                    // Auto accept
                    if (existing.SenderId == receiverId)
                    {
                        await AcceptFriendRequestAsync(receiverId, senderId);
                        return true;
                    }
                    // Existed request
                    else
                        throw new InvalidOperationException("Existed request");
                }

                // Be friend
                if (existing.Status == "A")
                    throw new InvalidOperationException("This user was a friend");

                // Resend request
                if (existing.Status == "R")
                {
                    existing.Status = "P";
                    existing.CreatedAt = DateTime.UtcNow;
                }
            }

            // 3. Create new request
            var newRequest = new FriendRequest
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = "P",
                CreatedAt = DateTime.UtcNow
            };

            _context.FriendRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
