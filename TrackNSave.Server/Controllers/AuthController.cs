using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TrackNSave.Server.Models;
using TrackNSave.Server.Services;
using TrackNSave.Server.Services.Interfaces;

namespace TrackNSave.Server.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public AuthController(IConfiguration config, IUserService userService, IPasswordService passwordService, IJwtService jwtService)
        {
            _userService = userService;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var errorMessage = await _userService.RegisterUserAsync(request.Username, request.Email, request.Password);
            if (errorMessage != null)
            {
                return StatusCode(409, new { message = errorMessage });
            }

            return StatusCode(200, new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserByUsernameAsync(request.Username);
            if (user == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return StatusCode(401, new { message = "Invalid username or password" });
            }

            string token = _jwtService.GenerateToken(user);

            Response.Cookies.Append("auth_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return StatusCode(200, new { token });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("auth_token");
            return StatusCode(200, new { message = "Logout successful" });
        }
    }
}