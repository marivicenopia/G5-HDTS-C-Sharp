using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class TicketAttachmentService : ITicketAttachmentService
    {
        private readonly ITicketAttachmentRepository _attachmentRepo;
        private readonly ITicketRepository _ticketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TicketAttachmentService(ITicketAttachmentRepository attachmentRepo, ITicketRepository ticketRepo, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _attachmentRepo = attachmentRepo;
            _ticketRepo = ticketRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketAttachmentDto>> GetByTicketAsync(string ticketId)
        {
            var list = _attachmentRepo.GetByTicket(ticketId).ToList();
            return _mapper.Map<IEnumerable<TicketAttachmentDto>>(list);
        }

        public async Task<TicketAttachmentDto> AddAsync(string ticketId, CreateTicketAttachmentDto dto)
        {
            var ticket = await _ticketRepo.GetTicketByIdAsync(ticketId);
            if (ticket == null) return null;
            var entity = new TicketAttachment
            {
                Id = Guid.NewGuid().ToString(),
                TicketId = ticketId,
                Name = dto.Name,
                Size = dto.Size,
                Type = dto.Type,
                UploadDate = DateTime.UtcNow,
                Url = dto.Url
            };
            _attachmentRepo.Add(entity);
            _unitOfWork.SaveChanges();
            return _mapper.Map<TicketAttachmentDto>(entity);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await _attachmentRepo.GetByIdAsync(id);
            if (entity == null) return false;
            _attachmentRepo.Remove(entity);
            _unitOfWork.SaveChanges();
            return true;
        }
    }
}
