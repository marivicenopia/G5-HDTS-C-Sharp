using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;
        public AuthController(IUserService userService, IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
            Console.WriteLine("✅ AuthController dependencies resolved successfully!");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AuthenticateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid request data");
                }

                // Use the existing authentication method from UserService
                var authResult = await _userService.AuthenticateUserAsync(request.Username, request.Password);
                if (authResult != Resources.Constants.Enums.LoginResult.Success)
                {
                    return Unauthorized("Invalid username or password");
                }

                // Get user details
                var user = await _userService.GetUserByUsernameAsync(request.Username);
                if (user == null || !user.IsActive)
                {
                    return Unauthorized("Account is deactivated or not found");
                }

                // Generate JWT token
                var token = _jwtTokenService.GenerateToken(user);

                return Ok(new
                {
                    status = "Success",
                    response = new
                    {
                        userId = user.UserId,
                        username = user.Username,
                        email = user.Email,
                        role = user.Role,
                        departmentId = user.DepartmentId,
                        fullName = $"{user.FirstName} {user.LastName}".Trim(),
                        isActive = user.IsActive,
                        token = token
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Login error: {ex.Message}");
                return StatusCode(500, "An error occurred during login");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Clear server-side session if using sessions (session might be disabled)
                try
                {
                    HttpContext.Session?.Clear();
                }
                catch (InvalidOperationException)
                {
                    // Session is not configured, which is fine for JWT authentication
                }

                // Clear specific known authentication cookies
                Response.Cookies.Delete("tkn");
                Response.Cookies.Delete("authToken");
                Response.Cookies.Delete("asi.basecode"); // Session cookie
                Response.Cookies.Delete(Resources.Constants.Const.Issuer); // Session cookie by constant

                // Clear all authentication-related cookies
                foreach (var cookieName in Request.Cookies.Keys)
                {
                    if (cookieName.Contains("auth") ||
                        cookieName.ToLower().Contains("token") ||
                        cookieName.ToLower().Contains("basecode") ||
                        cookieName == "tkn" ||
                        cookieName == "asi.basecode")
                    {
                        Response.Cookies.Delete(cookieName);
                    }
                }

                Console.WriteLine("✅ User logged out successfully on server side");
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Logout error: {ex.Message}");
                return StatusCode(500, "An error occurred during logout");
            }
        }
    }

}
