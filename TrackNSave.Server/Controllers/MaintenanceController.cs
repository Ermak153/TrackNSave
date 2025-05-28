using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackNSave.Server.Models.DTOs;
using TrackNSave.Server.Services.Interfaces;
using System.Security.Claims;

namespace TrackNSave.Server.Controllers
{
    [ApiController]
    [Route("api/maintenance/")]
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetMaintenanceStatus()
        {
            try
            {
                var isEnabled = await _maintenanceService.IsMaintenanceModeAsync();
                return StatusCode(200, new { isEnabled });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error getting maintenance status" });
            }
        }

        [HttpPost("toggle")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleMaintenanceMode()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return StatusCode(401, new { message = "User is not authenticated" });
            }

            try
            {
                await _maintenanceService.ToggleMaintenanceModeAsync();
                var newStatus = await _maintenanceService.IsMaintenanceModeAsync();
                return StatusCode(200, new
                {
                    success = true,
                    isEnabled = newStatus,
                    message = $"Maintenance mode {(newStatus ? "enabled" : "disabled")} successfully"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error toggling maintenance mode" });
            }
        }

        [HttpPost("emergency-toggle")]
        public async Task<IActionResult> EmergencyToggleMaintenanceMode([FromBody] EmergencyMaintenanceRequest request)
        {
            if (string.IsNullOrEmpty(request?.Token))
            {
                return StatusCode(400, new { message = "Token is required" });
            }

            try
            {
                var success = await _maintenanceService.EmergencyToggleMaintenanceModeAsync(request.Token);
                
                if (!success)
                {
                    return StatusCode(401, new { message = "Invalid token" });
                }

                var newStatus = await _maintenanceService.IsMaintenanceModeAsync();
                return StatusCode(200, new { 
                    success = true, 
                    isEnabled = newStatus, 
                    message = $"Emergency maintenance mode {(newStatus ? "enabled" : "disabled")} successfully" 
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error during emergency toggle" });
            }
        }
    }
}
