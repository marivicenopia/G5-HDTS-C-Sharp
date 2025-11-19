using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous] // Temporarily allow anonymous access for development
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase<UserController>
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public UserController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IUserService userService,
            IUserRepository userRepository) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _userService = userService;
            _userRepository = userRepository;
        }

        /// Get all users - For frontend compatibility with mock API
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                var userList = users.Select(user => new
                {
                    id = user.Id,
                    userId = user.UserId,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role,
                    departmentId = user.DepartmentId,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    isActive = user.IsActive,
                    createdTime = user.CreatedTime,
                    // Note: Not returning password for security
                    password = "***" // Placeholder - never return actual passwords
                }).ToList();

                return Ok(new ApiResult<object>(Status.Success, userList, "Users retrieved successfully"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while retrieving users"));
            }
        }

        /// Get user by ID - Returns full user data for editing
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new ApiResult<object>(Status.Error, null, "User not found"));
                }

                var response = new
                {
                    id = user.Id,
                    userId = user.UserId,
                    username = user.Username,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    role = user.Role,
                    departmentId = user.DepartmentId,
                    isActive = user.IsActive,
                    createdTime = user.CreatedTime,
                    updatedTime = user.UpdatedTime
                };

                return Ok(new ApiResult<object>(Status.Success, response, "User found"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error getting user: {UserId}", userId);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while retrieving user information"));
            }
        }

        /// Check if user exists by username
        [HttpGet("exists/{username}")]
        public async Task<IActionResult> UserExists(string username)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);
                var exists = user != null;

                return Ok(new ApiResult<bool>(Status.Success, exists, exists ? "User exists" : "User does not exist"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error checking if user exists: {Username}", username);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while checking user existence"));
            }
        }

        /// Get users by role
        [HttpGet("role/{role}")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            try
            {
                var users = await _userService.GetUsersByRoleAsync(role);

                var userList = users.Select(user => new
                {
                    id = user.Id,
                    userId = user.UserId,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role,
                    departmentId = user.DepartmentId,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    isActive = user.IsActive,
                    createdTime = user.CreatedTime
                }).ToList();

                return Ok(new ApiResult<object>(Status.Success, userList, "Users retrieved successfully"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error retrieving users by role: {Role}", role);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while retrieving users"));
            }
        }

        /// Get users by department
        [HttpGet("department/{department}")]
        public async Task<IActionResult> GetUsersByDepartment(string department)
        {
            try
            {
                var users = await _userService.GetUsersByDepartmentAsync(department);

                var userList = users.Select(user => new
                {
                    id = user.Id,
                    userId = user.UserId,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role,
                    departmentId = user.DepartmentId,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    isActive = user.IsActive,
                    createdTime = user.CreatedTime
                }).ToList();

                return Ok(new ApiResult<object>(Status.Success, userList, "Users retrieved successfully"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error retrieving users by department: {Department}", department);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while retrieving users"));
            }
        }

        /// Create new user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new ApiResult<object>(Status.Error, errors, "Invalid request data"));
                }

                // Check if username already exists
                var existingUser = await _userService.GetUserByUsernameAsync(request.Username);
                if (existingUser != null)
                {
                    return BadRequest(new ApiResult<object>(Status.Error, null, "Username already exists"));
                }

                var user = new User
                {
                    // Let UserService generate sequential ID
                    // UserId will be set by the service if needed
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Role = request.Role,
                    DepartmentId = request.DepartmentId,
                    IsActive = true
                };

                var createdUser = await _userService.CreateUserAsync(user);

                var response = new
                {
                    id = createdUser.Id,
                    userId = createdUser.UserId,
                    username = createdUser.Username,
                    email = createdUser.Email,
                    role = createdUser.Role,
                    departmentId = createdUser.DepartmentId,
                    firstName = createdUser.FirstName,
                    lastName = createdUser.LastName,
                    isActive = createdUser.IsActive,
                    createdTime = createdUser.CreatedTime
                };

                return Ok(new ApiResult<object>(Status.Success, response, "User created successfully"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error creating user: {ErrorMessage}. Request: {@Request}", ex.Message, request);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, $"An error occurred while creating user: {ex.Message}"));
            }
        }

        /// Update user
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new ApiResult<object>(Status.Error, errors, "Invalid request data"));
                }

                var user = new User
                {
                    UserId = userId,
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password, // Will only be updated if provided
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Role = request.Role,
                    DepartmentId = request.DepartmentId,
                    IsActive = request.IsActive
                };

                var updatedUser = await _userService.UpdateUserAsync(user);

                if (updatedUser == null)
                {
                    return NotFound(new ApiResult<object>(Status.Error, null, "User not found"));
                }

                var response = new
                {
                    id = updatedUser.Id,
                    userId = updatedUser.UserId,
                    username = updatedUser.Username,
                    email = updatedUser.Email,
                    role = updatedUser.Role,
                    departmentId = updatedUser.DepartmentId,
                    firstName = updatedUser.FirstName,
                    lastName = updatedUser.LastName,
                    isActive = updatedUser.IsActive,
                    createdTime = updatedUser.CreatedTime,
                    updatedTime = updatedUser.UpdatedTime
                };

                return Ok(new ApiResult<object>(Status.Success, response, "User updated successfully"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error updating user: {UserId}", userId);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while updating user"));
            }
        }

        /// Delete user
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var success = await _userService.DeleteUserAsync(userId);

                if (!success)
                {
                    return NotFound(new ApiResult<object>(Status.Error, null, "User not found"));
                }

                return Ok(new ApiResult<object>(Status.Success, null, "User deleted successfully"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error deleting user: {UserId}", userId);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while deleting user"));
            }
        }

        /// Deactivate user
        [HttpPatch("{userId}/deactivate")]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            try
            {
                var success = await _userService.DeactivateUserAsync(userId);

                if (!success)
                {
                    return NotFound(new ApiResult<object>(Status.Error, null, "User not found"));
                }

                return Ok(new ApiResult<object>(Status.Success, null, "User deactivated successfully"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error deactivating user: {UserId}", userId);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while deactivating user"));
            }
        }

        /// Activate user
        [HttpPatch("{userId}/activate")]
        public async Task<IActionResult> ActivateUser(string userId)
        {
            try
            {
                var success = await _userService.ActivateUserAsync(userId);

                if (!success)
                {
                    return NotFound(new ApiResult<object>(Status.Error, null, "User not found"));
                }

                return Ok(new ApiResult<object>(Status.Success, null, "User activated successfully"));
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex, "Error activating user: {UserId}", userId);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while activating user"));
            }
        }
    }
}
