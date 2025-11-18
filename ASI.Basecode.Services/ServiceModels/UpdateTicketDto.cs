using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    /// <summary>
    /// DTO for updating an existing Ticket
    /// </summary>
    public class UpdateTicketDto
    {
  [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
  public string Description { get; set; }

   [StringLength(50, ErrorMessage = "Priority cannot exceed 50 characters")]
        public string Priority { get; set; }

        [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters")]
     public string Department { get; set; }

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }

        [StringLength(100, ErrorMessage = "AssignedTo cannot exceed 100 characters")]
        public string AssignedTo { get; set; }

        [StringLength(100, ErrorMessage = "ResolvedBy cannot exceed 100 characters")]
   public string ResolvedBy { get; set; }

        public DateTime? ResolvedDate { get; set; }

        [StringLength(2000, ErrorMessage = "ResolutionDescription cannot exceed 2000 characters")]
        public string ResolutionDescription { get; set; }

        [StringLength(2000, ErrorMessage = "AgentFeedback cannot exceed 2000 characters")]
        public string AgentFeedback { get; set; }
    }
}
