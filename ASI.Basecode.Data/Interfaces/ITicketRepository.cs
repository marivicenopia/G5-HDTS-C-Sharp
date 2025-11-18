using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
  /// <summary>
    /// Interface for Ticket Repository operations
    /// </summary>
    public interface ITicketRepository
    {
        /// <summary>
    /// Get all tickets
        /// </summary>
        /// <returns>IQueryable of Ticket</returns>
        IQueryable<Ticket> GetTickets();

        /// <summary>
        /// Get a ticket by ID
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <returns>Ticket or null if not found</returns>
     Task<Ticket> GetTicketByIdAsync(string id);

        /// <summary>
        /// Add a new ticket
    /// </summary>
   /// <param name="ticket">Ticket to add</param>
        void Add(Ticket ticket);

        /// <summary>
        /// Update an existing ticket
/// </summary>
        /// <param name="ticket">Ticket to update</param>
    void Update(Ticket ticket);

        /// <summary>
        /// Delete a ticket
        /// </summary>
        /// <param name="id">Ticket ID</param>
        Task<bool> DeleteAsync(string id);
    }
}
