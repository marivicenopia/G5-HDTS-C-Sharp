using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersControllerOLd : ControllerBase
    {
        private readonly NexDeskDbContext _context;

        public UsersControllerOLd(NexDeskDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                // Remove password from response for security
                var userResponse = users.Select(u => new
                {
                    u.Id,
                    u.UserId,
                    u.Username,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Role,
                    u.Department,
                    u.CreatedBy,
                    u.CreatedTime,
                    u.UpdatedTime
                });
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Remove password from response for security
                var userResponse = new
                {
                    user.Id,
                    user.UserId,
                    user.Username,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Role,
                    user.Department,
                    user.CreatedBy,
                    user.CreatedTime,
                    user.UpdatedTime
                };
                
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            try
            {
                // Generate a unique ID for the user
                user.Id = Guid.NewGuid().ToString("N")[..10].ToUpper();
                user.CreatedTime = DateTime.Now;
                
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Remove password from response for security
                var userResponse = new
                {
                    user.Id,
                    user.UserId,
                    user.Username,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Role,
                    user.Department,
                    user.CreatedBy,
                    user.CreatedTime,
                    user.UpdatedTime
                };

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create user", error = ex.Message });
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            try
            {
                user.UpdatedTime = DateTime.Now;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                
                // Remove password from response for security
                var userResponse = new
                {
                    user.Id,
                    user.UserId,
                    user.Username,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Role,
                    user.Department,
                    user.CreatedBy,
                    user.CreatedTime,
                    user.UpdatedTime
                };
                
                return Ok(userResponse);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound(new { message = "User not found" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update user", error = ex.Message });
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete user", error = ex.Message });
            }
        }

        // GET: api/users/by-role/{role}
        [HttpGet("by-role/{role}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(string role)
        {
            try
            {
                var users = await _context.Users
                    .Where(u => u.Role.ToLower() == role.ToLower())
                    .ToListAsync();
                    
                var userResponse = users.Select(u => new
                {
                    u.Id,
                    u.UserId,
                    u.Username,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Role,
                    u.Department,
                    u.CreatedBy,
                    u.CreatedTime,
                    u.UpdatedTime
                });
                
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/users/by-department/{department}
        [HttpGet("by-department/{department}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByDepartment(string department)
        {
            try
            {
                var users = await _context.Users
                    .Where(u => u.Department.ToLower() == department.ToLower())
                    .ToListAsync();
                    
                var userResponse = users.Select(u => new
                {
                    u.Id,
                    u.UserId,
                    u.Username,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Role,
                    u.Department,
                    u.CreatedBy,
                    u.CreatedTime,
                    u.UpdatedTime
                });
                
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}