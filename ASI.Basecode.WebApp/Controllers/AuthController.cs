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
    [Route("api/[controller]/[action]")]
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

                // Generate JWT token (if implemented)
                // var token = await _jwtTokenService.GenerateTokenAsync(user);

                return Ok(new
                {
                    // token, // Uncomment when JWT service is implemented
                    role = user.Role,
                    username = user.Username,
                    departmentId = user.DepartmentId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Login error: {ex.Message}");
                return StatusCode(500, "An error occurred during login");
            }
        }
    }

}
