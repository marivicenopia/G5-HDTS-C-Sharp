using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
 public class CreateTicketAttachmentDto
 {
 [Required]
 public string Name { get; set; }
 public int? Size { get; set; }
 public string Type { get; set; }
 public string Url { get; set; }
 }
}