using System;

namespace ASI.Basecode.Services.ServiceModels
{
 public class TicketAttachmentDto
 {
 public string Id { get; set; }
 public string TicketId { get; set; }
 public string Name { get; set; }
 public int? Size { get; set; }
 public string Type { get; set; }
 public DateTime? UploadDate { get; set; }
 public string Url { get; set; }
 }
}