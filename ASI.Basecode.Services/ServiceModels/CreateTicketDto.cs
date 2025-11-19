using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    /// <summary>
    /// DTO for creating a new Ticket
    /// </summary>
    public class CreateTicketDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [StringLength(10, ErrorMessage = "Priority cannot exceed 10 characters")]
        public string Priority { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters")]
        public string Department { get; set; }

        [Required(ErrorMessage = "SubmittedBy is required")]
        [StringLength(100, ErrorMessage = "SubmittedBy cannot exceed 100 characters")]
        public string SubmittedBy { get; set; }

        // Optional in request; backend will default if missing
        [StringLength(100, ErrorMessage = "AssignedTo cannot exceed 100 characters")]
        public string AssignedTo { get; set; }
    }
}
