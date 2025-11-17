using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASI.Basecode.WebApp.Models.NexDesk
{
    [Table("Feedbacks")]
    public class Feedback
    {
        [Key]
        [Column("Id")]
        [StringLength(30)]
        public string Id { get; set; }

        [Column("Name")]
        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [Column("Email")]
        [StringLength(100)]
        [Required]
        public string Email { get; set; }

        [Column("Title")]
        [StringLength(100)]
        [Required]
        public string Title { get; set; }

        [Column("Message")]
        public string Message { get; set; }

        [Column("Experience")]
        [StringLength(10)]
        [Required]
        public string Experience { get; set; }  // "Good", "Bad", "Average"

        [Column("Date")]
        public DateTime Date { get; set; }

        [Column("TicketId")]
        [StringLength(20)]
        [Required]
        public string TicketId { get; set; }
    }
}