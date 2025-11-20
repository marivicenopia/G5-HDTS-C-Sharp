using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ASI.Basecode.WebApp.Controllers
{
    public class TicketAttachmentController: ControllerBase<TicketAttachmentController>
    {
        private readonly ITicketAttachmentService _ticketAttachmentService;
        public TicketAttachmentController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            ITicketAttachmentService ticketAttachmentService) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _ticketAttachmentService = ticketAttachmentService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
