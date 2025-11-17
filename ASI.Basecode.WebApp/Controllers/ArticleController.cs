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
    public class ArticleController: ControllerBase<ArticleController>
    {
        private readonly IArticleService _articleService;
        public ArticleController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IArticleService articleService) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _articleService = articleService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
