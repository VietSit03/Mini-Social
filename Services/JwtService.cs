using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniSocialAPI.Models.Entities;
using MiniSocialAPI.Services.Interfaces;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MiniSocialAPI.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly IDatabase _redis;

        public JwtService(IConfiguration configuration, IConnectionMultiplexer redis)
        {
            _config = configuration;
            _redis = redis.GetDatabase();
        }

        public string GenerateAccessToken(User user)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
            };

            var expireDate = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["Jwt:Expire"]));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expireDate,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task SaveRefreshTokenAsync(User user, string refreshToken, TimeSpan ttl)
        {
            string key = $"refresh:{user.Id}:{refreshToken}";
            await _redis.StringSetAsync(key, "valid", ttl);
        }

        public async Task<bool> ValidateRefreshTokenAsync(User user, string refreshToken)
        {
            string key = $"refresh:{user.Id}:{refreshToken}";
            return await _redis.KeyExistsAsync(key);
        }
    }
}
