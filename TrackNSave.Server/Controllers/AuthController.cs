using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TrackNSave.Server.Models;
using TrackNSave.Server.Services;

namespace TrackNSave.Server.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _config;

        public AuthController(UserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var errorMessage = await _userService.RegisterUserAsync(request.Username, request.Email, request.Password);
            if (errorMessage != null)
            {
                return Conflict(new { message = errorMessage });
            }

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserByUsernameAsync(request.Username);
            if (user == null || !_userService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            string token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email)
            };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
