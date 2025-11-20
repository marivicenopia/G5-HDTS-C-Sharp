using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASI.Basecode.WebApp.Models.NexDesk
{
    [Table("Tickets")]
    public class Ticket
    {
        [Key]
        [Column("Id")]
        [StringLength(10)]
        public string Id { get; set; }

        [Column("Title")]
        [StringLength(50)]
        [Required]
        public string Title { get; set; }

        [Column("Description")]
        [Required]
        public string Description { get; set; }

        [Column("Priority")]
        [StringLength(10)]
        [Required]
        public string Priority { get; set; }  // "Low", "Medium", "High"

        [Column("Department")]
        [StringLength(50)]
        [Required]
        public string Department { get; set; }

        [Column("SubmittedBy")]
        [StringLength(100)]
        [Required]
        public string SubmittedBy { get; set; }

        [Column("SubmittedDate")]
        public DateTime SubmittedDate { get; set; }

        [Column("Status")]
        [StringLength(20)]
        [Required]
        public string Status { get; set; }  // "Open", "In Progress", "Resolved", "Closed"

        [Column("AssignedTo")]
        [StringLength(100)]
        [Required]
        public string AssignedTo { get; set; }

        [Column("ResolvedBy")]
        [StringLength(100)]
        [Required]
        public string ResolvedBy { get; set; }

        [Column("ResolvedDate")]
        public DateTime ResolvedDate { get; set; }

        [Column("ResolutionDescription")]
        public string ResolutionDescription { get; set; }

        [Column("AgentFeedback")]
        public string AgentFeedback { get; set; }
    }
}