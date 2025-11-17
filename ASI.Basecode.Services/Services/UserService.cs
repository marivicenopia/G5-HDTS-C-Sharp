using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;

        public UserService(IUserRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IDepartmentService departmentService)
        {
            _mapper = mapper;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _departmentService = departmentService;
        }

        /// <summary>
        /// Generates a department-based UserId (e.g., IT001, HR002, FIN003)
        /// </summary>
        private async Task<string> GenerateDepartmentBasedUserIdAsync(string departmentId)
        {
            // Department ID to prefix mapping
            var departmentPrefixes = new Dictionary<string, string>
            {
                { "1", "IT" },      // IT Department
                { "2", "HR" },      // HR Department  
                { "3", "FIN" },     // Finance Department
                { "4", "MKT" },     // Marketing Department
                { "5", "OPS" },     // Operations Department
                { "6", "CS" }       // Customer Support Department
            };

            // Get the department prefix
            if (!departmentPrefixes.TryGetValue(departmentId, out string prefix))
            {
                // Fallback to generic prefix if department not found
                prefix = "USR";
            }

            // Get all existing users in this department
            var existingUsers = await _repository.GetUsers()
                .Where(u => u.DepartmentId == departmentId)
                .ToListAsync();

            // Find the highest sequential number for this department prefix
            var maxSequence = 0;
            var prefixPattern = $"{prefix}(\\d{{3}})"; // Matches IT001, HR002, etc.
            var regex = new System.Text.RegularExpressions.Regex(prefixPattern);

            foreach (var existingUser in existingUsers)
            {
                if (!string.IsNullOrEmpty(existingUser.UserId))
                {
                    var match = regex.Match(existingUser.UserId);
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int sequence))
                    {
                        if (sequence > maxSequence)
                        {
                            maxSequence = sequence;
                        }
                    }
                }
            }

            // Generate the next UserId with zero-padded sequence
            var nextSequence = maxSequence + 1;
            var generatedUserId = $"{prefix}{nextSequence:D3}"; // D3 formats as 3-digit zero-padded number
            Console.WriteLine($"DEBUG: GenerateDepartmentBasedUserIdAsync - Department: {departmentId}, Prefix: {prefix}, Max Sequence: {maxSequence}, Generated: {generatedUserId}");
            return generatedUserId;
        }

        public LoginResult AuthenticateUser(string userId, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.UserId == userId &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _repository.GetUsers()
                .Where(x => x.Username == username)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _repository.GetUsers()
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            var passwordKey = PasswordManager.EncryptPassword(password);
            var user = await _repository.GetUsers()
                .Where(x => x.Username == username && x.Password == passwordKey && x.IsActive)
                .FirstOrDefaultAsync();

            return user != null;
        }

        public async Task<LoginResult> AuthenticateUserAsync(string username, string password)
        {
            var passwordKey = PasswordManager.EncryptPassword(password);
            var user = await _repository.GetUsers()
                .Where(x => x.Username == username && x.Password == passwordKey)
                .FirstOrDefaultAsync();

            if (user == null)
                return LoginResult.Failed;

            if (!user.IsActive)
                return LoginResult.Failed; // Could add a new enum value for inactive users

            return LoginResult.Success;
        }

        // User Management Implementation
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _repository.GetUsers()
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _repository.GetUsers()
                .Where(x => x.Role == role)
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByDepartmentAsync(string departmentId)
        {
            return await _repository.GetUsers()
                .Where(x => x.DepartmentId == departmentId)
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            // Validate name formats (no numbers allowed)
            if (!string.IsNullOrEmpty(user.FirstName))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(user.FirstName, @"\d"))
                {
                    throw new ArgumentException("First name cannot contain numbers. Please use only letters, spaces, and common name characters.");
                }
            }

            if (!string.IsNullOrEmpty(user.LastName))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(user.LastName, @"\d"))
                {
                    throw new ArgumentException("Last name cannot contain numbers. Please use only letters, spaces, and common name characters.");
                }
            }

            // Check if username already exists
            if (await UserExistsAsync(user.Username))
            {
                throw new ArgumentException($"Username '{user.Username}' is already taken. Please choose a different username.");
            }

            // Validate password complexity if password is provided
            if (!string.IsNullOrEmpty(user.Password))
            {
                var passwordValidation = PasswordManager.ValidatePassword(user.Password, user.Username);
                if (!passwordValidation.IsValid)
                {
                    throw new ArgumentException($"Password validation failed: {passwordValidation.ErrorMessage}");
                }

                // Check for password uniqueness (not same as existing users)
                var existingUsers = await _repository.GetUsers().ToListAsync();
                var encryptedPassword = PasswordManager.EncryptPassword(user.Password);

                if (existingUsers.Any(u => u.Password == encryptedPassword))
                {
                    throw new ArgumentException("This password is already in use by another account. Please choose a unique password.");
                }
            }

            // Generate sequential ID for the primary key
            var allUsers = await _repository.GetUsers().ToListAsync();
            var maxId = 0;

            // Find the highest existing numeric ID (primary key)
            foreach (var existingUser in allUsers)
            {
                if (int.TryParse(existingUser.Id, out int id) && id > maxId)
                {
                    maxId = id;
                }
            }

            // Set the new ID as the next sequential number (this is the primary key)
            user.Id = (maxId + 1).ToString();

            // Ensure required fields are set with default values if empty
            if (string.IsNullOrEmpty(user.DepartmentId))
            {
                // Get the first active department as default
                var departments = await _departmentService.GetActiveDepartmentsAsync();
                var defaultDept = departments.FirstOrDefault();
                user.DepartmentId = defaultDept?.Id.ToString() ?? "1"; // Fallback to IT department ID if no departments found
            }
            else
            {
                // Validate that the provided department ID exists
                var departments = await _departmentService.GetActiveDepartmentsAsync();
                var validDepartment = departments.Any(d => d.Id.Equals(user.DepartmentId, StringComparison.OrdinalIgnoreCase));
                if (!validDepartment)
                {
                    throw new ArgumentException($"Invalid department ID: {user.DepartmentId}. Please select from available departments.");
                }
            }

            // Generate department-based UserId (e.g., IT001, HR002, FIN003)
            if (string.IsNullOrEmpty(user.UserId))
            {
                user.UserId = await GenerateDepartmentBasedUserIdAsync(user.DepartmentId);
            }

            if (string.IsNullOrEmpty(user.Role))
            {
                user.Role = "Staff"; // Default role
            }

            // Encrypt password before saving
            if (!string.IsNullOrEmpty(user.Password))
            {
                user.Password = PasswordManager.EncryptPassword(user.Password);
            }

            // Ensure field length constraints are met
            user.Username = user.Username?.Substring(0, Math.Min(user.Username.Length, 30));
            user.Email = user.Email?.Substring(0, Math.Min(user.Email.Length, 50));
            user.FirstName = user.FirstName?.Substring(0, Math.Min(user.FirstName.Length, 50));
            user.LastName = user.LastName?.Substring(0, Math.Min(user.LastName.Length, 50));
            user.DepartmentId = user.DepartmentId?.Substring(0, Math.Min(user.DepartmentId.Length, 450)); // DepartmentId is nvarchar(450)
            user.Role = user.Role?.Substring(0, Math.Min(user.Role.Length, 50));
            user.UserId = user.UserId?.Substring(0, Math.Min(user.UserId.Length, 50));

            user.CreatedTime = DateTime.Now;
            user.UpdatedTime = DateTime.Now;
            user.IsActive = true;
            user.CreatedBy = "System"; // Max 50 chars, fits constraint

            // Debug: Check UserId before and after database save
            Console.WriteLine($"DEBUG: UserId before database save: '{user.UserId}'");
            Console.WriteLine($"DEBUG: Username: '{user.Username}', Department: '{user.DepartmentId}'");
            
            await _unitOfWork.Database.Set<User>().AddAsync(user);
            await _unitOfWork.Database.SaveChangesAsync();
            
            Console.WriteLine($"DEBUG: UserId after database save: '{user.UserId}'");
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            // Validate name formats (no numbers allowed)
            if (!string.IsNullOrEmpty(user.FirstName))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(user.FirstName, @"\d"))
                {
                    throw new ArgumentException("First name cannot contain numbers. Please use only letters, spaces, and common name characters.");
                }
            }

            if (!string.IsNullOrEmpty(user.LastName))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(user.LastName, @"\d"))
                {
                    throw new ArgumentException("Last name cannot contain numbers. Please use only letters, spaces, and common name characters.");
                }
            }

            var existingUser = await _repository.GetUsers()
                .Where(x => x.UserId == user.UserId)
                .FirstOrDefaultAsync();

            if (existingUser == null)
                return null;

            // Update fields
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Role = user.Role;
            existingUser.DepartmentId = user.DepartmentId;
            existingUser.IsActive = user.IsActive;

            // Only update password if provided
            if (!string.IsNullOrEmpty(user.Password))
            {
                existingUser.Password = PasswordManager.EncryptPassword(user.Password);
            }

            existingUser.UpdatedTime = DateTime.Now;

            _unitOfWork.Database.Set<User>().Update(existingUser);
            await _unitOfWork.Database.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _repository.GetUsers()
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            if (user == null)
                return false;

            _unitOfWork.Database.Set<User>().Remove(user);
            await _unitOfWork.Database.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            var user = await _repository.GetUsers()
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            if (user == null)
                return false;

            user.IsActive = false;
            user.UpdatedTime = DateTime.Now;

            _unitOfWork.Database.Set<User>().Update(user);
            await _unitOfWork.Database.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateUserAsync(string userId)
        {
            var user = await _repository.GetUsers()
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            if (user == null)
                return false;

            user.IsActive = true;
            user.UpdatedTime = DateTime.Now;

            _unitOfWork.Database.Set<User>().Update(user);
            await _unitOfWork.Database.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var user = await _repository.GetUsers()
                .Where(x => x.Username == username)
                .FirstOrDefaultAsync();

            return user != null;
        }
    }
}