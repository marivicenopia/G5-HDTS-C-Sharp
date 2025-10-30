using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly NexDeskDbContext _context;

        public TicketsController(NexDeskDbContext context)
        {
            _context = context;
        }

        // GET: api/tickets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            try
            {
                var tickets = await _context.Tickets.ToListAsync();
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/tickets/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(string id)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(id);
                if (ticket == null)
                {
                    return NotFound(new { message = "Ticket not found" });
                }
                return Ok(ticket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // POST: api/tickets
        [HttpPost]
        public async Task<ActionResult<Ticket>> CreateTicket(Ticket ticket)
        {
            try
            {
                // Generate a unique ID for the ticket
                ticket.Id = Guid.NewGuid().ToString("N")[..10].ToUpper();
                ticket.SubmittedDate = DateTime.Now;
                
                // Set default values if not provided
                if (string.IsNullOrEmpty(ticket.Status))
                    ticket.Status = "Open";
                
                if (string.IsNullOrEmpty(ticket.Priority))
                    ticket.Priority = "Medium";

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create ticket", error = ex.Message });
            }
        }

        // PUT: api/tickets/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(string id, Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            try
            {
                _context.Entry(ticket).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(ticket);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
                {
                    return NotFound(new { message = "Ticket not found" });
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update ticket", error = ex.Message });
            }
        }

        // DELETE: api/tickets/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(string id)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(id);
                if (ticket == null)
                {
                    return NotFound(new { message = "Ticket not found" });
                }

                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Ticket deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete ticket", error = ex.Message });
            }
        }

        // PATCH: api/tickets/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateTicketStatus(string id, [FromBody] string status)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(id);
                if (ticket == null)
                {
                    return NotFound(new { message = "Ticket not found" });
                }

                ticket.Status = status;
                ticket.ResolvedDate = status.ToLower() == "resolved" ? DateTime.Now : (DateTime?)null;

                await _context.SaveChangesAsync();
                return Ok(ticket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update ticket status", error = ex.Message });
            }
        }

        // PATCH: api/tickets/{id}/assign
        [HttpPatch("{id}/assign")]
        public async Task<IActionResult> AssignTicket(string id, [FromBody] string assignedTo)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(id);
                if (ticket == null)
                {
                    return NotFound(new { message = "Ticket not found" });
                }

                ticket.AssignedTo = assignedTo;
                await _context.SaveChangesAsync();
                return Ok(ticket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to assign ticket", error = ex.Message });
            }
        }

        private bool TicketExists(string id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}