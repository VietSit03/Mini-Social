using Microsoft.AspNetCore.Mvc;
using MiniSocialAPI.Services.Interfaces;
using System.Security.Claims;

namespace MiniSocialAPI.MiniSocialAPI.Controllers
{
    public class FriendshipController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FriendshipController(IFriendshipService friendshipService, IHttpContextAccessor httpContextAccessor)
        {
            _friendshipService = friendshipService;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/friendship
        [HttpGet]
        public async Task<IActionResult> GetFriends()
        {
            var userId = GetCurrentUserId();
            var friends = await _friendshipService.GetFriendsAsync(userId);
            return Ok(friends);
        }

        // GET: api/friendship/pending
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var userId = GetCurrentUserId();
            var requests = await _friendshipService.GetPendingRequestsAsync(userId, "");
            return Ok(requests);
        }

        // POST: api/friendship/request
        [HttpPost("request")]
        public async Task<IActionResult> SendRequest([FromBody] Guid receiverId)
        {
            var senderId = GetCurrentUserId();
            var result = await _friendshipService.SendFriendRequestAsync(senderId, receiverId);
            return Ok(result);
        }

        // PUT: api/friendship/accept
        [HttpPut("accept")]
        public async Task<IActionResult> AcceptRequest([FromBody] Guid senderId)
        {
            var receiverId = GetCurrentUserId();
            await _friendshipService.AcceptFriendRequestAsync(senderId, receiverId);
            return NoContent();
        }

        // PUT: api/friendship/reject
        [HttpPut("reject")]
        public async Task<IActionResult> RejectRequest([FromBody] Guid senderId)
        {
            var receiverId = GetCurrentUserId();
            await _friendshipService.RejectFriendRequestAsync(senderId, receiverId);
            return NoContent();
        }

        // DELETE: api/friendship/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveFriend(Guid userId)
        {
            var currentUserId = GetCurrentUserId();
            await _friendshipService.RemoveFriendAsync(currentUserId, userId);
            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userId);
        }
    }
}
