using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class TicketAttachmentRepository : BaseRepository, ITicketAttachmentRepository
    {
        public TicketAttachmentRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public IQueryable<TicketAttachment> GetByTicket(string ticketId)
        {
            return this.GetDbSet<TicketAttachment>().Where(a => a.TicketId == ticketId);
        }

        public async Task<TicketAttachment> GetByIdAsync(string id)
        {
            return await GetDbSet<TicketAttachment>().FirstOrDefaultAsync(a => a.Id == id);
        }

        public void Add(TicketAttachment attachment)
        {
            this.GetDbSet<TicketAttachment>().Add(attachment);
        }

        public void Remove(TicketAttachment attachment)
        {
            this.GetDbSet<TicketAttachment>().Remove(attachment);
        }
    }
}
