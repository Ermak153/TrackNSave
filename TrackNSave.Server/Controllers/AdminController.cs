using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackNSave.Server.Models;
using TrackNSave.Server.Services.Implementations;
using TrackNSave.Server.Services.Interfaces;
using TrackNSave.Server.Models.DTOs;

namespace TrackNSave.Server.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IReceiptService _receiptService;

        public AdminController(IUserService userService, IReceiptService receiptService)
        {
            _userService = userService;
            _receiptService = receiptService;
        }

        [HttpPost("users-list")]
        public async Task<IActionResult> GetUsers([FromBody] GetUsersRequest request)
        {
            try
            {
                var page = request.Page > 0 ? request.Page : 1;
                var pageSize = request.PageSize > 0 ? request.PageSize : 10;

                var (users, totalCount) = await _userService.GetAllUsersAsync(page, pageSize);

                var response = new
                {
                    Users = users.Select(u => new
                    {
                        u.Id,
                        u.Username,
                        u.Email,
                        u.CreatedAt,
                        Role = u.Role?.Name
                    }),
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving users", error = ex.Message });
            }
        }

        [HttpPut("user-edit")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(request.UserId, request.Username, request.Email);

                if (result)
                {
                    return StatusCode(200, new { message = "User updated successfully" });
                }

                return StatusCode(400, new { message = "Failed to update user" });
            }
            catch (UserServiceException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating user", error = ex.Message });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetAdminStats()
        {
            try
            {
                var (users, totalUsers) = await _userService.GetAllUsersAsync(1, int.MaxValue);
                var totalReceipts = await _receiptService.GetTotalReceiptsCountAsync();
                var totalAmount = await _receiptService.GetTotalAmountAsync();

                var stats = new
                {
                    TotalUsers = totalUsers,
                    TotalReceipts = totalReceipts,
                    TotalAmount = Math.Round(totalAmount, 2)
                };

                return StatusCode(200, stats);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error retrieving stats"});
            }
        }
    }
}
