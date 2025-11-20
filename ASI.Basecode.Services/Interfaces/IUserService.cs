using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string userid, string password, ref User user);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(string userId);
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
        Task<LoginResult> AuthenticateUserAsync(string username, string password);

        // User Management Methods
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<User>> GetUsersByDepartmentAsync(string department);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> ActivateUserAsync(string userId);
        Task<bool> UserExistsAsync(string username);
    }
}