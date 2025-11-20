using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ITicketAttachmentService
    {
        Task<IEnumerable<TicketAttachmentDto>> GetByTicketAsync(string ticketId);
        Task<TicketAttachmentDto> AddAsync(string ticketId, CreateTicketAttachmentDto dto);
        Task<bool> DeleteAsync(string id);
    }
}
