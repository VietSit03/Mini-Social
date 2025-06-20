using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MiniSocialAPI.Models;
using MiniSocialAPI.Models.Entities;
using MiniSocialAPI.Models.Requests;
using MiniSocialAPI.Models.Responses;
using MiniSocialAPI.Services;
using MiniSocialAPI.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MiniSocialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, IJwtService jwtService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Models.Requests.LoginRequest request)
        {
            try
            {
                var user = await _userService.AuthenticateAsync(request.Username, request.Password);
                if (user == null)
                    return Unauthorized(new { message = "Wrong information" });

                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();
                await _jwtService.SaveRefreshTokenAsync(user, refreshToken, TimeSpan.FromDays(7));

                return Ok(new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] OAuthRequest model)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken);
                if (payload == null || !payload.EmailVerified)
                    return Unauthorized(new { message = "Invalid token" });

                var user = await _userService.GetByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = await _userService.CreateUserFromOAuthAsync(payload.Email, payload.Name, payload.Picture);
                }

                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();
                await _jwtService.SaveRefreshTokenAsync(user, refreshToken, TimeSpan.FromDays(7));

                return Ok(new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var user = await _userService.GetByIdAsync(request.UserId);
                if (user == null)
                    return Unauthorized(new { message = "Invalid user" });

                var valid = await _jwtService.ValidateRefreshTokenAsync(user, request.RefreshToken);
                if (!valid)
                    return Unauthorized(new { message = "Invalid refresh token" });

                var newAccessToken = _jwtService.GenerateAccessToken(user);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                // Token rotation: save new token, not remove old token because it will automatically expire
                await _jwtService.SaveRefreshTokenAsync(user, newRefreshToken, TimeSpan.FromDays(7));

                return Ok(new AuthResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            try
            {
                var user = await _userService.GetByUsernameAsync(request.Username);
                if (user != null)
                    return BadRequest(new { message = "User existed" });

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Email = request.Email,
                    FullName = request.FullName,
                    CreatedAt = DateTime.UtcNow,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                };

                await _userService.CreateAsync(newUser);

                var accessToken = _jwtService.GenerateAccessToken(newUser);
                var refreshToken = _jwtService.GenerateRefreshToken();
                await _jwtService.SaveRefreshTokenAsync(newUser, refreshToken, TimeSpan.FromDays(7));

                return Ok(new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
