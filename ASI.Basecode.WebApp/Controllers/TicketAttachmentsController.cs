using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace ASI.Basecode.WebApp.Controllers
{
 [ApiController]
 [Route("api/tickets/{ticketId}/attachments-table")]
 [AllowAnonymous]
 public class TicketAttachmentsController : ControllerBase
 {
 private readonly ITicketAttachmentService _attachmentService;
 public TicketAttachmentsController(ITicketAttachmentService attachmentService)
 { _attachmentService = attachmentService; }

 [HttpGet]
 public async Task<IActionResult> List(string ticketId)
 {
 var list = await _attachmentService.GetByTicketAsync(ticketId);
 return Ok(list);
 }

 [HttpPost]
 [RequestSizeLimit(50_000_000)]
 public async Task<IActionResult> Upload(string ticketId, [FromForm] IFormFile file)
 {
 // Fallback: accept any key by reading Request.Form.Files
 if ((file == null || file.Length ==0) && Request?.Form?.Files?.Count >0)
 {
 file = Request.Form.Files[0];
 }
 if (file == null || file.Length ==0) return BadRequest(new { message = "Empty file" });
 var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", ticketId);
 Directory.CreateDirectory(uploadsDir);
 var newName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
 var fullPath = Path.Combine(uploadsDir, newName);
 using (var stream = System.IO.File.Create(fullPath))
 { await file.CopyToAsync(stream); }
 var dto = new CreateTicketAttachmentDto
 {
 Name = file.FileName,
 Size = (int)file.Length,
 Type = file.ContentType,
 Url = $"/uploads/{ticketId}/{newName}"
 };
 var saved = await _attachmentService.AddAsync(ticketId, dto);
 if (saved == null) return NotFound(new { message = "Ticket not found" });
 return Created(saved.Url, saved);
 }

 [HttpPost("batch")]
 [RequestSizeLimit(100_000_000)]
 public async Task<IActionResult> UploadBatch(string ticketId, [FromForm] List<IFormFile> files)
 {
 // Fallback: if key name differs, pull from Request.Form.Files
 if ((files == null || files.Count ==0) && Request?.Form?.Files?.Count >0)
 {
 files = Request.Form.Files.ToList();
 }
 if (files == null || files.Count ==0) return BadRequest(new { message = "No files provided" });
 var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", ticketId);
 Directory.CreateDirectory(uploadsDir);
 var results = new List<TicketAttachmentDto>();
 foreach (var f in files.Where(f => f?.Length >0))
 {
 var newName = Guid.NewGuid().ToString() + Path.GetExtension(f.FileName);
 var fullPath = Path.Combine(uploadsDir, newName);
 using (var stream = System.IO.File.Create(fullPath))
 { await f.CopyToAsync(stream); }
 var dto = new CreateTicketAttachmentDto
 {
 Name = f.FileName,
 Size = (int)f.Length,
 Type = f.ContentType,
 Url = $"/uploads/{ticketId}/{newName}"
 };
 var saved = await _attachmentService.AddAsync(ticketId, dto);
 if (saved != null) results.Add(saved);
 }
 if (results.Count ==0) return NotFound(new { message = "Ticket not found or no files saved" });
 return Created($"/api/tickets/{ticketId}/attachments-table", results);
 }

 [HttpDelete("{id}")]
 public async Task<IActionResult> Delete(string ticketId, string id)
 {
 var ok = await _attachmentService.DeleteAsync(id);
 if (!ok) return NotFound();
 return NoContent();
 }
 }
}
