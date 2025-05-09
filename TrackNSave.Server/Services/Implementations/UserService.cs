using TrackNSave.Server.Data;
using TrackNSave.Server.Models;
using TrackNSave.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TrackNSave.Server.Services.Implementations
{
    public class UserServiceException : Exception
    {
        public int StatusCode { get; }
        public UserServiceException(int statusCode, string message) : base(message) { StatusCode = statusCode; }
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IPasswordService _passwordService;

        public UserService(ApplicationDbContext context, IConfiguration config, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
            _config = config;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<string?> GetUserIdFromJwtAsync(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out _);

                var username = principal?.Identity?.Name;
                if (string.IsNullOrEmpty(username)) return null;

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                return user?.Id.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string?> RegisterUserAsync(string username, string email, string password)
        {
            var existingUser = await GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                return "Username already exists";
            }

            var existingEmail = await GetUserByEmailAsync(email);
            if (existingEmail != null)
            {
                return "Email already exists";
            }

            _passwordService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = DateTime.UtcNow,
                RoleId = 1
            };

            await AddUserAsync(newUser);

            return null;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username == user.Username || u.Email == user.Email))
                {
                    return false;
                }

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task UpdateUserAvatarAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (existingUser == null)
                {
                    throw new UserServiceException(404, "User was not found");
                }

                _context.Entry(existingUser).CurrentValues.SetValues(new
                {
                    user.AvatarFileName,
                });

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new UserServiceException(500, "Error updating user avatar");
            }
        }
    }
}