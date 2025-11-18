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
    /// <summary>
    /// Service for Ticket operations
    /// </summary>
    public class TicketService: ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all tickets asynchronously
        /// </summary>
        public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
        {
            var tickets = _ticketRepository.GetTickets().ToList();
            return _mapper.Map<IEnumerable<TicketDto>>(tickets);
        }

        /// <summary>
        /// Get a specific ticket by ID asynchronously
        /// </summary>
        public async Task<TicketDto> GetTicketByIdAsync(string id)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(id);
            if (ticket == null)
                return null;

            return _mapper.Map<TicketDto>(ticket);
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
    }
}
