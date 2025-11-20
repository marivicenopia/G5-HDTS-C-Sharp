using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    /// <summary>
    /// Interface for Ticket Service operations
    /// </summary>
    public interface ITicketService
    {
        /// <summary>
        /// Get all tickets asynchronously
        /// </summary>
        /// <returns>Collection of TicketDto</returns>
        Task<IEnumerable<TicketDto>> GetAllTicketsAsync();

        /// <summary>
        /// Get a specific ticket by ID asynchronously
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <returns>TicketDto or null if not found</returns>
        Task<TicketDto> GetTicketByIdAsync(string id);

        /// <summary>
        /// Create a new ticket asynchronously
        /// </summary>
        /// <param name="createTicketDto">Ticket creation data</param>
        /// <returns>Created TicketDto</returns>
        Task<TicketDto> CreateTicketAsync(CreateTicketDto createTicketDto);

        /// <summary>
        /// Create a new ticket with attachments asynchronously
        /// </summary>
        /// <param name="createTicketDto">Ticket creation data</param>
        /// <param name="attachments">The collection of attachment metadata to add</param>
        /// <returns>Created TicketDto with attachments</returns>
        Task<TicketDto> CreateTicketWithAttachmentsAsync(CreateTicketDto createTicketDto, IEnumerable<CreateTicketAttachmentDto> attachments);

        /// <summary>
        /// Update an existing ticket asynchronously
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <param name="updateTicketDto">Ticket update data</param>
        /// <returns>True if update successful, false otherwise</returns>
        Task<bool> UpdateTicketAsync(string id, UpdateTicketDto updateTicketDto);

        /// <summary>
        /// Delete a ticket asynchronously
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <returns>True if delete successful, false otherwise</returns>
        Task<bool> DeleteTicketAsync(string id);

        /// <summary>
        /// Add attachments metadata to a ticket asynchronously
        /// </summary>
        /// <param name="ticketId">The ID of the ticket to add attachments to</param>
        /// <param name="attachments">The collection of attachment metadata to add</param>
        /// <returns>True if the operation was successful, false otherwise</returns>
        Task<bool> AddAttachmentsAsync(string ticketId, IEnumerable<CreateTicketAttachmentDto> attachments);
    }
}
