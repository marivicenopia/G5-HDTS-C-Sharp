using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASI.Basecode.WebApp.Models.NexDesk
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("Id")]
        [StringLength(10)]
        public string Id { get; set; }

        [Column("FirstName")]
        [StringLength(50)]
        [Required]
        public string FirstName { get; set; }

        [Column("LastName")]
        [StringLength(50)]
        [Required]
        public string LastName { get; set; }

        [Column("Email")]
        [StringLength(50)]
        [Required]
        public string Email { get; set; }

        [Column("Username")]
        [StringLength(30)]
        [Required]
        public string Username { get; set; }

        [Column("Password")]
        [StringLength(255)]
        [Required]
        public string Password { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("Role")]
        [StringLength(50)]
        [Required]
        public string Role { get; set; }  // "Admin", "Agent", "User"

        [Column("Department")]
        [StringLength(50)]
        public string Department { get; set; }

        [Column("CreatedTime")]
        public DateTime CreatedTime { get; set; }

        [Column("CreatedBy")]
        [StringLength(50)]
        [Required]
        public string CreatedBy { get; set; }

        [Column("UpdatedTime")]
        public DateTime UpdatedTime { get; set; }

        [Column("UserId")]
        [StringLength(50)]
        [Required]
        public string UserId { get; set; }
    }
}