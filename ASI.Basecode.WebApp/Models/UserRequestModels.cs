using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Models
{
    public class CreateUserRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(64, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 64 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{}|;:,.<>?]).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^[^0-9]*$", ErrorMessage = "First name cannot contain numbers.")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[^0-9]*$", ErrorMessage = "Last name cannot contain numbers.")]
        public string LastName { get; set; }

        [Required]
        public string Role { get; set; }

        public string DepartmentId { get; set; }
    }

    public class UpdateUserRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Password is optional for updates
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^[^0-9]*$", ErrorMessage = "First name cannot contain numbers.")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[^0-9]*$", ErrorMessage = "Last name cannot contain numbers.")]
        public string LastName { get; set; }

        [Required]
        public string Role { get; set; }

        public string DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}