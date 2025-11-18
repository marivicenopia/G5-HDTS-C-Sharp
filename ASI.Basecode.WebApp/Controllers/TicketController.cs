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

        public TicketController(
       IHttpContextAccessor httpContextAccessor,
 ILoggerFactory loggerFactory,
            IConfiguration configuration,
       IMapper mapper,
            ITicketService ticketService) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _ticketService = ticketService;
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
