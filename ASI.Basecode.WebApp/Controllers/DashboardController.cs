using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase<DashboardController>
    {
        private readonly IDashboardService _dashboardService;
        private readonly IUserService _userService;

        public DashboardController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IDashboardService dashboardService,
            IUserService userService) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _dashboardService = dashboardService;
            _userService = userService;
        }

        /// <summary>
        /// Get complete dashboard data for the authenticated user
        /// </summary>
        [HttpGet("data")]
        [AllowAnonymous] // Temporarily remove auth for testing
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                // Temporarily hardcode user info for testing
                var data = await _dashboardService.GetDashboardDataAsync("admin", "test-user", "1");

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard data");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get dashboard statistics only for the authenticated user
        /// </summary>
        [HttpGet("stats")]
        [Authorize]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var userInfo = GetUserInfoFromToken();
                if (userInfo == null)
                {
                    return Unauthorized("Invalid token or user information not found");
                }

                var stats = await _dashboardService.GetDashboardStatsAsync(
                    userInfo.Role,
                    userInfo.UserId,
                    userInfo.DepartmentId);

                return Ok(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard statistics");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get dashboard data for a specific role/user (Admin only)
        /// </summary>
        [HttpGet("data/{role}")]
        [Authorize]
        public async Task<IActionResult> GetDashboardDataForRole(string role, [FromQuery] string userId = null, [FromQuery] string departmentId = null)
        {
            try
            {
                var currentUser = GetUserInfoFromToken();
                if (currentUser == null)
                {
                    return Unauthorized("Invalid token or user information not found");
                }

                // Only admins and superadmins can view data for other roles
                if (!IsAdminOrSuperAdmin(currentUser.Role))
                {
                    return Forbid("Insufficient permissions to access this data");
                }

                var data = await _dashboardService.GetDashboardDataAsync(role, userId, departmentId);

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard data for role {Role}", role);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        private UserInfo GetUserInfoFromToken()
        {
            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return null;
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var departmentId = jwtToken.Claims.FirstOrDefault(c => c.Type == "department")?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
                {
                    return null;
                }

                return new UserInfo
                {
                    UserId = userId,
                    Role = role,
                    DepartmentId = departmentId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting user information from token");
                return null;
            }
        }

        private bool IsAdminOrSuperAdmin(string role)
        {
            return role?.ToLower() == "admin" || role?.ToLower() == "superadmin";
        }

        private class UserInfo
        {
            public string UserId { get; set; }
            public string Role { get; set; }
            public string DepartmentId { get; set; }
        }
    }
}