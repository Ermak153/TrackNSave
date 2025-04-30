using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackNSave.Server.Models.DTOs;
using TrackNSave.Server.Services.Implementations;
using TrackNSave.Server.Services.Interfaces;

namespace TrackNSave.Server.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, IPasswordService passwordService, ITokenService tokenService)
        {
            _userService = userService;
            _passwordService = passwordService;
            _tokenService = tokenService;
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

            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.Name)
                };

                string accessToken = _tokenService.GenerateAccessToken(claims);

                string refreshToken = _tokenService.GenerateRefreshToken();

                await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken, TimeSpan.FromDays(7));

                Response.Cookies.Append("auth_token", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                });

                Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return StatusCode(200, new { token = accessToken });
            }
            catch (TokenServiceException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred during authorization" });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return StatusCode(400, new { message = "Refresh токен отсутствует" });
            }

            try
            {
                var (savedRefreshToken, userId) = await _tokenService.FindRefreshTokenAsync(refreshToken);

                if (savedRefreshToken == null || savedRefreshToken.ExpiryDate < DateTime.UtcNow)
                {
                    return StatusCode(400, new { message = "Недействительный refresh token" });
                }

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return StatusCode(400, new { message = "Пользователь не найден" });
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.Name)
                };

                var newAccessToken = _tokenService.GenerateAccessToken(claims);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                await _tokenService.RevokeRefreshTokenAsync(userId, refreshToken);

                await _tokenService.SaveRefreshTokenAsync(userId, newRefreshToken, TimeSpan.FromDays(7));

                Response.Cookies.Append("auth_token", newAccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                });

                Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return StatusCode(200, new { token = newAccessToken });
            }
            catch (TokenServiceException ex)
            {
                return StatusCode(ex.StatusCode, new {  message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(400, new { message = "Token refresh error" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var accessToken = Request.Cookies["auth_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    try
                    {
                        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
                        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                        if (Guid.TryParse(userIdClaim, out var userId))
                        {
                            await _tokenService.RevokeRefreshTokenAsync(userId, refreshToken);
                        }
                    }
                    catch (TokenServiceException ex)
                    {
                        return StatusCode(ex.StatusCode, new { message = ex.Message });
                    }
                    catch (Exception)
                    {
                        return StatusCode(500, "Error when trying to logout");
                    }
                }
            }

            Response.Cookies.Delete("auth_token");
            Response.Cookies.Delete("refresh_token");

            return StatusCode(200, new { message = "Logout successful" });
        }
    }
}