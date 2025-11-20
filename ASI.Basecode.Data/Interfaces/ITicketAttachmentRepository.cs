using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ITicketAttachmentRepository
    {
        IQueryable<TicketAttachment> GetByTicket(string ticketId);
        Task<TicketAttachment> GetByIdAsync(string id);
        void Add(TicketAttachment attachment);
        void Remove(TicketAttachment attachment);
    }
}
