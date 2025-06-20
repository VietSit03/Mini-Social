using MiniSocialAPI.Models.Entities;

namespace MiniSocialAPI.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        Task SaveRefreshTokenAsync(User user, string refreshToken, TimeSpan ttl);
        Task<bool> ValidateRefreshTokenAsync(User user, string refreshToken);
    }
}
