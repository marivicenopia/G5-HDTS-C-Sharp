using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    /// <summary>
    /// Repository for Ticket entity
    /// </summary>`
    public class TicketRepository: BaseRepository, ITicketRepository
    {
        public TicketRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        /// Get all tickets
        /// </summary>
        public IQueryable<Ticket> GetTickets()
        {
            return this.GetDbSet<Ticket>();
        }

        /// <summary>
        /// Get a ticket by ID asynchronously
        /// </summary>
        public async Task<Ticket> GetTicketByIdAsync(string id)
        {
            return await GetDbSet<Ticket>().FirstOrDefaultAsync(t => t.Id == id);
        }

        /// <summary>
        /// Add a new ticket
        /// </summary>
        public void Add(Ticket ticket)
        {
            this.GetDbSet<Ticket>().Add(ticket);
        }

        /// <summary>
        /// Update an existing ticket
        /// </summary>
        public void Update(Ticket ticket)
        {
            this.SetEntityState(ticket, EntityState.Modified);
        }

        /// <summary>
        /// Delete a ticket asynchronously
        /// </summary>
        public async Task<bool> DeleteAsync(string id)
        {
            var ticket = await GetTicketByIdAsync(id);
            if (ticket == null)
                return false;

            this.GetDbSet<Ticket>().Remove(ticket);
            return true;
        }
    }
}
