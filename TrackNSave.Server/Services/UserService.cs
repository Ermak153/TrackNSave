using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using TrackNSave.Server.Data;
using TrackNSave.Server.Models;

namespace TrackNSave.Server.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA256(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
            }
        }

        public async Task<string?> RegisterUserAsync(string username, string email, string password)
        {
            if (!Regex.IsMatch(username, "^[a-zA-Z0-9]+$"))
                return "Username can contain only Latin letters and numbers";

            if (!new EmailAddressAttribute().IsValid(email))
                return "Invalid email format";

            if (password.Length < 5)
                return "Password must be at least 5 characters long";

            if (await GetUserByUsernameAsync(username) != null)
                return "Username already taken";

            if (await GetUserByEmailAsync(email) != null)
                return "Email already taken";

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return null;
        }

    }
}
