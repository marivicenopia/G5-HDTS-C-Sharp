using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ASI.Basecode.WebApp.Models
{
    /// <summary>
    /// Login Response Model
    /// </summary>
    public class LoginResponseModel
    {
        /// <summary>User ID</summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        /// <summary>User Email</summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>User Role</summary>
        [JsonPropertyName("role")]
        public string Role { get; set; }

        /// <summary>User Department ID</summary>
        [JsonPropertyName("departmentId")]
        public string DepartmentId { get; set; }

        /// <summary>User Full Name</summary>
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        /// <summary>Is Active</summary>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        /// <summary>Authentication Token</summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }

    /// <summary>
    /// User Authentication Request
    /// </summary>
    public class AuthenticateUserRequest
    {
        /// <summary>Username</summary>
        [JsonPropertyName("username")]
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        /// <summary>Password</summary>
        [JsonPropertyName("password")]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}