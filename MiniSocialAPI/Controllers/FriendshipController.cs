using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using MiniSocialAPI.MiniSocialAPI.Middleware;
using MiniSocialAPI.Models.DTOs;
using MiniSocialAPI.Models.Responses;
using MiniSocialAPI.Services.Interfaces;
using System.Security.Claims;

namespace MiniSocialAPI.MiniSocialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipController : ControllerBase
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
        [CustomAuthorization]
        public async Task<IActionResult> GetFriends()
        {
            try
            {
                var userId = GetCurrentUserId();
                var friends = await _friendshipService.GetFriendsAsync(userId);
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Get friends list succeed",
                    Data = friends
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new FailureResponse(ex.Message));
            }
        }

        // GET: api/friendship/pending
        [HttpGet("pending")]
        [CustomAuthorization]
        public async Task<IActionResult> GetPendingRequests([FromQuery] string direction)
        {
            try
            {
                var userId = GetCurrentUserId();
                var requests = await _friendshipService.GetPendingRequestsAsync(userId, direction);
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Get pending requests list succeed",
                    Data = requests
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new FailureResponse(ex.Message));
            }
        }

        // POST: api/friendship/request
        [HttpPost("request")]
        [CustomAuthorization]
        public async Task<IActionResult> SendRequest([FromBody] Guid receiverId)
        {
            try
            {
                var senderId = GetCurrentUserId();
                var result = await _friendshipService.SendFriendRequestAsync(senderId, receiverId);
                return Ok(new SuccessResponse());
            }
            catch (Exception ex)
            {
                return BadRequest(new FailureResponse(ex.Message));
            }
        }

        // PUT: api/friendship/accept
        [HttpPut("accept")]
        [CustomAuthorization]
        public async Task<IActionResult> AcceptRequest([FromBody] Guid senderId)
        {
            try
            {
                var receiverId = GetCurrentUserId();
                await _friendshipService.AcceptFriendRequestAsync(senderId, receiverId);
                return Ok(new SuccessResponse());
            }
            catch (Exception ex)
            {
                return BadRequest(new FailureResponse(ex.Message));
            }
        }

        // PUT: api/friendship/reject
        [HttpPut("reject")]
        [CustomAuthorization]
        public async Task<IActionResult> RejectRequest([FromBody] Guid senderId)
        {
            try
            {
                var receiverId = GetCurrentUserId();
                await _friendshipService.RejectFriendRequestAsync(senderId, receiverId);
                return Ok(new SuccessResponse());
            }
            catch (Exception ex)
            {
                return BadRequest(new FailureResponse(ex.Message));
            }
        }

        // DELETE: api/friendship/{userId}
        [HttpDelete("{userId}")]
        [CustomAuthorization]
        public async Task<IActionResult> RemoveFriend(Guid userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                await _friendshipService.RemoveFriendAsync(currentUserId, userId);
                return Ok(new SuccessResponse());
            }
            catch (Exception ex)
            {
                return BadRequest(new FailureResponse(ex.Message));
            }
        }

        private Guid GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userId);
        }
    }
}
