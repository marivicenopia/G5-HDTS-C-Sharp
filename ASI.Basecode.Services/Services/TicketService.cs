using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ASI.Basecode.Services.Services
{
    /// <summary>
    /// Service for Ticket operations
    /// </summary>
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITicketAttachmentService _ticketAttachmentService;

        public TicketService(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IMapper mapper, ITicketAttachmentService ticketAttachmentService)
        {
            _ticketRepository = ticketRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ticketAttachmentService = ticketAttachmentService;
        }

        /// <summary>
        /// Get all tickets asynchronously
        /// </summary>
        public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
        {
            var tickets = _ticketRepository.GetTickets().ToList();
            var ticketDtos = new List<TicketDto>();

            foreach (var ticket in tickets)
            {
                var ticketDto = _mapper.Map<TicketDto>(ticket);

                // Get attachments for this ticket
                var attachments = await _ticketAttachmentService.GetByTicketAsync(ticket.Id);
                if (attachments.Any())
                {
                    ticketDto.AttachmentsJson = JsonConvert.SerializeObject(attachments);
                }

                ticketDtos.Add(ticketDto);
            }

            return ticketDtos;
        }

        /// <summary>
        /// Get a specific ticket by ID asynchronously
        /// </summary>
        public async Task<TicketDto> GetTicketByIdAsync(string id)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(id);
            if (ticket == null)
                return null;

            var ticketDto = _mapper.Map<TicketDto>(ticket);

            // Get attachments for this ticket
            var attachments = await _ticketAttachmentService.GetByTicketAsync(ticket.Id);
            if (attachments.Any())
            {
                ticketDto.AttachmentsJson = JsonConvert.SerializeObject(attachments);
            }

            return ticketDto;
        }

        /// <summary>
        /// Create a new ticket asynchronously
        /// </summary>
        public async Task<TicketDto> CreateTicketAsync(CreateTicketDto createTicketDto)
        {
            var ticket = _mapper.Map<Ticket>(createTicketDto);
            ticket.Id = Guid.NewGuid().ToString();
            ticket.SubmittedDate = DateTime.Now;
            ticket.Status = "Open";

            // Defaults for required columns in DB
            ticket.AssignedTo = string.IsNullOrWhiteSpace(ticket.AssignedTo) ? "Unassigned" : ticket.AssignedTo;
            ticket.ResolvedBy = string.IsNullOrWhiteSpace(ticket.ResolvedBy) ? "Unassigned" : ticket.ResolvedBy;
            ticket.ResolvedDate = ticket.ResolvedDate == default ? DateTime.Now : ticket.ResolvedDate;

            _ticketRepository.Add(ticket);
            _unitOfWork.SaveChanges();

            return _mapper.Map<TicketDto>(ticket);
        }

        /// <summary>
        /// Create a new ticket with attachments asynchronously
        /// </summary>
        public async Task<TicketDto> CreateTicketWithAttachmentsAsync(CreateTicketDto createTicketDto, IEnumerable<CreateTicketAttachmentDto> attachments)
        {
            var created = await CreateTicketAsync(createTicketDto);
            // attachments persisted by controller via attachment service; no JSON storage now
            return created;
        }

        /// <summary>
        /// Update an existing ticket asynchronously
        /// </summary>
        public async Task<bool> UpdateTicketAsync(string id, UpdateTicketDto updateTicketDto)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(id);
            if (ticket == null)
                return false;

            _mapper.Map(updateTicketDto, ticket);
            _ticketRepository.Update(ticket);
            _unitOfWork.SaveChanges();

            return true;
        }

        /// <summary>
        /// Delete a ticket asynchronously
        /// </summary>
        public async Task<bool> DeleteTicketAsync(string id)
        {
            var success = await _ticketRepository.DeleteAsync(id);
            if (success)
            {
                _unitOfWork.SaveChanges();
            }
            return success;
        }

        public Task<bool> AddAttachmentsAsync(string ticketId, IEnumerable<CreateTicketAttachmentDto> attachments)
        {
            // Deprecated; attachments handled by TicketAttachmentService directly.
            return Task.FromResult(true);
        }
    }
}
