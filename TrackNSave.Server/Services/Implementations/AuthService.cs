using Microsoft.AspNetCore.Identity;
using TrackNSave.Server.Models;
using TrackNSave.Server.Services.Interfaces;

namespace TrackNSave.Server.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;

        public AuthService(IUserService userService, IJwtService jwtService, IPasswordService passwordService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        public async Task<string?> AuthenticateAsync(string username, string password)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null || !_passwordService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return _jwtService.GenerateToken(user);
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            if (await _userService.GetUserByUsernameAsync(username) != null ||
                await _userService.GetUserByEmailAsync(email) != null)
                return false;

            _passwordService.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = DateTime.UtcNow
            };

            await _userService.AddUserAsync(user);
            return true;
        }
    }
}