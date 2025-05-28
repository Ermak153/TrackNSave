using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TrackNSave.Server.Services.Interfaces;
using TrackNSave.Server.Services.Implementations;
using TrackNSave.Server.Models.DTOs;

namespace TrackNSave.Server.Controllers
{
    [ApiController]
    [Route("api/user/")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IAvatarService _avatarService;

        public UserController(IUserService userService, ITokenService tokenService, IAvatarService avatarService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _avatarService = avatarService;
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
                role = roleName,
                registrationDate = user.CreatedAt
            });
        }

        [HttpGet("get-avatar")]
        [Authorize]
        public async Task<IActionResult?> GetUserAvatar()
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

            var avatar = await _avatarService.GetAvatarAsync(user.Id, user.AvatarFileName);
            if (avatar == null)
            {
                return StatusCode(204);
            }

            return avatar;
        }

        [HttpPost("upload-avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
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

            try
            {
                if (!string.IsNullOrEmpty(user.AvatarFileName))
                {
                    await _avatarService.DeleteAvatarAsync(user.Id, user.AvatarFileName);
                }

                var newAvatarFileName = await _avatarService.SaveAvatarAsync(file, user.Id);
                user.AvatarFileName = newAvatarFileName;
                await _userService.UpdateUserAvatarAsync(user);

                return StatusCode(200, new { avatarFileName = newAvatarFileName });
            }
            catch (AvatarServiceException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
            catch (UserServiceException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error updating the user's avatar" });
            }
        }

        [HttpDelete("delete-avatar")]
        [Authorize]
        public async Task<IActionResult> DeleteAvatar()
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

            try
            {
                if (!string.IsNullOrEmpty(user.AvatarFileName))
                {
                    await _avatarService.DeleteAvatarAsync(user.Id, user.AvatarFileName);
                    user.AvatarFileName = null;
                    await _userService.UpdateUserAvatarAsync(user);
                }

                return StatusCode(200, new { message = "Avatar deleted successfully" });
            }
            catch (AvatarServiceException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
            catch (UserServiceException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "" });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return StatusCode(401, new { message = "User is not authenticated" });
            }

            if (model.CurrentPassword == model.NewPassword)
            {
                return StatusCode(400, new { message = "The new password matches the current one" });
            }

            try
            {
                var result = await _userService.ChangePasswordAsync(username, model.CurrentPassword, model.NewPassword);
                if (!result)
                {
                    return StatusCode(400, new { message = "Current password is incorrect" });
                }

                return StatusCode(200, new { message = "Password changed successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error changing password"});
            }
        }
    }
}