using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KnowledgeBaseController : ControllerBase
    {
        private readonly IKnowledgeBaseService _knowledgeBaseService;

        public KnowledgeBaseController(IKnowledgeBaseService knowledgeBaseService)
        {
            _knowledgeBaseService = knowledgeBaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _knowledgeBaseService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle(string id)
        {
            var article = await _knowledgeBaseService.GetArticleAsync(id);
            if (article == null)
            {
                return NotFound(new { message = "Article not found" });
            }
            return Ok(article);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddArticle([FromBody] CreateArticleRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var currentUser = User?.Identity?.Name ?? "system";
                var createdArticle = await _knowledgeBaseService.AddArticleAsync(request, currentUser);
                return CreatedAtAction(nameof(GetArticle), new { id = createdArticle.Id }, createdArticle);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteArticle(string id)
        {
            try
            {
                var role = GetCurrentUserRole();
                await _knowledgeBaseService.DeleteArticleAsync(id, role);
                return Ok(new { message = "Article deleted successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Article not found" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private string GetCurrentUserRole()
        {
            return User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";
        }
    }
}

