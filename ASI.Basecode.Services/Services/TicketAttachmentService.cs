using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class TicketAttachmentService: ITicketAttachmentService
    {
        private readonly ITicketAttachmentRepository _ticketAttachmentRepository;
        public TicketAttachmentService(ITicketAttachmentRepository ticketAttachmentRepository)
        {
            _ticketAttachmentRepository = ticketAttachmentRepository;
        }
    }
}
