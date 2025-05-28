using TrackNSave.Server.Models;

namespace TrackNSave.Server.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<string?> GetUserIdFromJwtAsync(string token);
        Task<string?> RegisterUserAsync(string username, string email, string password);
        Task<bool> AddUserAsync(User user);
        Task UpdateUserAvatarAsync(User user);
        Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword);
        Task<(List<User> Users, int TotalCount)> GetAllUsersAsync(int page = 1, int pageSize = 10);
        Task<bool> UpdateUserAsync(Guid userId, string username, string email);
    }
}