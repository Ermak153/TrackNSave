using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TrackNSave.Server.Services;
using TrackNSave.Server.Services.Interfaces;

namespace TrackNSave.Server.Controllers
{
    [ApiController]
    [Route("api/user/")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public UserController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpGet("status")]
        public IActionResult CheckAuthStatus()
        {
            var token = Request.Cookies["auth_token"];
            if (string.IsNullOrEmpty(token))
            {
                return StatusCode(401, new { isAuthenticated = false });
            }

            if (_tokenService.ValidateToken(token))
            {
                return StatusCode(200, new { isAuthenticated = true });
            }

            Response.Cookies.Delete("auth_token");
            return StatusCode(401, new { isAuthenticated = false });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return StatusCode(401, new { message = "User is not authenticated" });
            }

            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return StatusCode(404, new { message = "User was not found" });
            }

            var roleName = user.Role?.Name ?? "User";

            return StatusCode(200, new
            {
                username = user.Username,
                email = user.Email,
                role = roleName
            });
        }
    }
}