using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Models
{
 public class CreateTicketFormRequest
 {
 [Required]
 [StringLength(50)]
 public string Title { get; set; }

 [Required]
 [StringLength(2000)]
 public string Description { get; set; }

 [Required]
 [StringLength(10)]
 public string Priority { get; set; }

 [Required]
 [StringLength(50)]
 public string Department { get; set; }

 [Required]
 [StringLength(100)]
 public string SubmittedBy { get; set; }

 [StringLength(100)]
 public string AssignedTo { get; set; }

 // Optional attachment file
 public IFormFile File { get; set; }
 }
}