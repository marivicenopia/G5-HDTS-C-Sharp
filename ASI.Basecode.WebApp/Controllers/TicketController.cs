using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// REST API Controller for Ticket operations
    /// </summary>
    [ApiController]
    [Route("api/tickets")]
    [AllowAnonymous]
    public class TicketController : ControllerBase<TicketController>
    {
        private readonly ITicketService _ticketService;
        private readonly ITicketAttachmentService _attachmentService; // added

        public TicketController(
       IHttpContextAccessor httpContextAccessor,
 ILoggerFactory loggerFactory,
            IConfiguration configuration,
       IMapper mapper,
            ITicketService ticketService,
 ITicketAttachmentService attachmentService) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _ticketService = ticketService;
 _attachmentService = attachmentService;
  }

        /// <summary>
        /// Get all tickets
        /// </summary>
        /// <returns>List of all tickets</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetAllTickets()
      {
       try
  {
       var tickets = await _ticketService.GetAllTicketsAsync();
        return Ok(tickets);
        }
     catch (Exception ex)
            {
          this.HandleExceptionLog(ex, nameof(GetAllTickets));
       return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving tickets." });
       }
      }

        /// <summary>
        /// Get a ticket by ID
        /// </summary>
    /// <param name="id">Ticket ID</param>
        /// <returns>Ticket details or 404 if not found</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
 public async Task<ActionResult<TicketDto>> GetTicketById(string id)
        {
            try
            {
   var ticket = await _ticketService.GetTicketByIdAsync(id);
   if (ticket == null)
             {
           return NotFound(new { message = $"Ticket with ID {id} not found." });
            }
 return Ok(ticket);
            }
  catch (Exception ex)
  {
              this.HandleExceptionLog(ex, nameof(GetTicketById));
     return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the ticket." });
 }
        }

        /// <summary>
     /// Create a new ticket
    /// </summary>
        /// <param name="createTicketDto">Ticket creation data</param>
      /// <returns>Created ticket with 201 status</returns>
        [HttpPost]
 [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] CreateTicketDto createTicketDto)
        {
   try
      {
    if (!ModelState.IsValid)
     {
       return BadRequest(ModelState);
     }

        var createdTicket = await _ticketService.CreateTicketAsync(createTicketDto);
   return CreatedAtAction(nameof(GetTicketById), new { id = createdTicket.Id }, createdTicket);
 }
       catch (Exception ex)
       {
 this.HandleExceptionLog(ex, nameof(CreateTicket));
          return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the ticket.", error = ex.Message, innerException = ex.InnerException?.Message });
   }
        }

        /// <summary>
        /// Update an existing ticket
     /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <param name="updateTicketDto">Ticket update data</param>
   /// <returns>204 NoContent on success or 404 if not found</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
     [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> UpdateTicket(string id, [FromBody] UpdateTicketDto updateTicketDto)
        {
      try
            {
   if (!ModelState.IsValid)
        {
        return BadRequest(ModelState);
  }

         var ticketExists = await _ticketService.GetTicketByIdAsync(id);
     if (ticketExists == null)
          {
          return NotFound(new { message = $"Ticket with ID {id} not found." });
         }

     await _ticketService.UpdateTicketAsync(id, updateTicketDto);
   return NoContent();
            }
    catch (Exception ex)
{
       this.HandleExceptionLog(ex, nameof(UpdateTicket));
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the ticket." });
}
        }

   /// <summary>
        /// Delete a ticket
  /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <returns>204 NoContent on success or 404 if not found</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTicket(string id)
{
        try
            {
                var ticketExists = await _ticketService.GetTicketByIdAsync(id);
         if (ticketExists == null)
       {
   return NotFound(new { message = $"Ticket with ID {id} not found." });
      }

                await _ticketService.DeleteTicketAsync(id);
    return NoContent();
            }
        catch (Exception ex)
            {
  this.HandleExceptionLog(ex, nameof(DeleteTicket));
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the ticket." });
        }
        }

        /// <summary>
 /// Append attachment metadata directly into the Ticket.AttachmentsJson column (no separate table).
 /// Form file field: files (multiple) or file (single).
 /// </summary>
 [HttpPost("{id}/attachments-json")]
 [Consumes("multipart/form-data")]
 [ProducesResponseType(StatusCodes.Status200OK)]
 [ProducesResponseType(StatusCodes.Status404NotFound)]
 public async Task<IActionResult> AddAttachmentsJson(string id, [FromForm] List<IFormFile> files, [FromForm] IFormFile file)
 {
 try
 {
 var ticket = await _ticketService.GetTicketByIdAsync(id);
 if (ticket == null)
 return NotFound(new { message = $"Ticket with ID {id} not found." });

 if ((files == null || files.Count ==0) && file != null)
 files = new List<IFormFile> { file };
 if (files == null || files.Count ==0)
 return BadRequest(new { message = "No files provided" });

 var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", id);
 Directory.CreateDirectory(uploadsDir);
 var metaList = new List<CreateTicketAttachmentDto>();
 foreach (var f in files.Where(f => f != null && f.Length >0))
 {
 var newName = Guid.NewGuid() + Path.GetExtension(f.FileName);
 var fullPath = Path.Combine(uploadsDir, newName);
 using (var stream = System.IO.File.Create(fullPath))
 await f.CopyToAsync(stream);
 metaList.Add(new CreateTicketAttachmentDto
 {
 Name = f.FileName,
 Size = (int)f.Length,
 Type = f.ContentType,
 Url = $"/uploads/{id}/{newName}"
 });
 }
 var ok = await _ticketService.AddAttachmentsAsync(id, metaList);
 if (!ok) return NotFound(new { message = "Ticket not found (during update)" });
 var updated = await _ticketService.GetTicketByIdAsync(id);
 return Ok(updated);
 }
 catch (Exception ex)
 {
 this.HandleExceptionLog(ex, nameof(AddAttachmentsJson));
 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error adding attachments", error = ex.Message });
 }
 }

        /// <summary>
 /// Single-step ticket creation with optional attachments (multipart/form-data).
 /// Form fields: title, description, priority, department, submittedBy, assignedTo(optional)
 /// Files: file (single) or files (multiple)
 /// Returns ticket plus saved attachments.
 /// </summary>
 [HttpPost("with-attachments")]
 [Consumes("multipart/form-data")]
 [ProducesResponseType(StatusCodes.Status201Created)]
 [ProducesResponseType(StatusCodes.Status400BadRequest)]
 public async Task<IActionResult> CreateTicketWithAttachments(
 [FromForm] CreateTicketDto createTicketDto,
 [FromForm] List<IFormFile> files,
 [FromForm] IFormFile file)
 {
 try
 {
 if (!ModelState.IsValid) return BadRequest(ModelState);
 var createdTicket = await _ticketService.CreateTicketAsync(createTicketDto);
 // Normalize files list
 if ((files == null || files.Count ==0) && file != null)
 files = new List<IFormFile> { file };
 var saved = new List<TicketAttachmentDto>();
 if (files != null && files.Count >0)
 {
 var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", createdTicket.Id);
 Directory.CreateDirectory(uploadDir);
 foreach (var f in files.Where(f => f != null && f.Length >0))
 {
 var newName = Guid.NewGuid() + Path.GetExtension(f.FileName);
 var fullPath = Path.Combine(uploadDir, newName);
 using (var stream = System.IO.File.Create(fullPath))
 await f.CopyToAsync(stream);
 var meta = new CreateTicketAttachmentDto
 {
 Name = f.FileName,
 Size = (int)f.Length,
 Type = f.ContentType,
 Url = $"/uploads/{createdTicket.Id}/{newName}"
 };
 var added = await _attachmentService.AddAsync(createdTicket.Id, meta);
 if (added != null) saved.Add(added);
 }
 }
 return CreatedAtAction(nameof(GetTicketById), new { id = createdTicket.Id }, new { ticket = createdTicket, attachments = saved });
 }
 catch (Exception ex)
 {
 this.HandleExceptionLog(ex, nameof(CreateTicketWithAttachments));
 return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating ticket with attachments", error = ex.Message });
 }
 }
    }
}

//// Map CreateTicketDto to Ticket
//CreateMap<CreateTicketDto, Ticket>()
//    .ForMember(dest => dest.Id, opt => opt.Ignore())
//    .ForMember(dest => dest.SubmittedDate, opt => opt.Ignore())
//    .ForMember(dest => dest.Status, opt => opt.Ignore())
//    /* ... other ignored fields ... */;

//// Map UpdateTicketDto to Ticket  
//CreateMap<UpdateTicketDto, Ticket>()
//    .ForMember(dest => dest.Id, opt => opt.Ignore())
//    /* ... other ignored fields ... */;

//// Map Ticket to TicketDto
//CreateMap<Ticket, TicketDto>();
