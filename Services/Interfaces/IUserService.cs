using MiniSocialAPI.Models.Entities;

namespace MiniSocialAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
        Task<User> CreateUserFromOAuthAsync(string email, string name, string picture);
        Task CreateAsync(User user);
    }
}
