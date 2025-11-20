using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/KnowledgeBase")]
    [ApiController]
    public class KnowledgeBaseController : ControllerBase<KnowledgeBaseController>
    {
        private readonly IArticleService _articleService;

        public KnowledgeBaseController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IArticleService articleService) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _articleService = articleService;
        }

        /// <summary>
        /// Get all categories with their articles
        /// </summary>
        [HttpGet("GetCategories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var articles = await _articleService.GetAllArticlesAsync();

                // Group articles by category
                var categories = articles
                    .GroupBy(a => a.Category ?? "Uncategorized")
                    .Select(g => new
                    {
                        categoryId = g.Key,
                        name = g.Key,
                        description = "",
                        displayOrder = 0,
                        articles = g.Select(a => new
                        {
                            id = a.Id,
                            title = a.Title,
                            author = a.Author,
                            status = "ACTIVE"
                        }).ToList()
                    })
                    .OrderBy(c => c.name)
                    .ToList();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while retrieving categories"));
            }
        }

        /// <summary>
        /// Get article by ID
        /// </summary>
        [HttpGet("GetArticle/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetArticle(string id)
        {
            try
            {
                var article = await _articleService.GetArticleByIdAsync(id);

                if (article == null)
                {
                    return NotFound(new ApiResult<object>(Status.Error, null, "Article not found"));
                }

                var response = new
                {
                    id = article.Id,
                    title = article.Title,
                    content = article.Content,
                    author = article.Author,
                    categoryId = article.Category,
                    categoryName = article.Category
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error retrieving article: {Id}", id);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while retrieving article"));
            }
        }

        /// <summary>
        /// Add new article
        /// </summary>
        [HttpPost("AddArticle")]
        [AllowAnonymous]
        public async Task<IActionResult> AddArticle([FromBody] AddArticleRequest request)
        {
            try
            {
                var errors = new List<string>();
                var title = request.Title?.Trim();
                var category = request.CategoryId?.Trim();
                var content = request.Content?.Trim();
                var author = request.Author?.Trim();

                if (string.IsNullOrWhiteSpace(category))
                {
                    errors.Add("Please select a category.");
                }

                if (string.IsNullOrWhiteSpace(title))
                {
                    errors.Add("Title is required.");
                }
                else if (title.Length > 100)
                {
                    errors.Add("Title must not exceed 100 characters.");
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    errors.Add("Content is required.");
                }
                else if (content.Length < 50)
                {
                    errors.Add("Content must be at least 50 characters.");
                }

                if (errors.Any())
                {
                    return BadRequest(new ApiResult<object>(Status.Error, errors, "Please fix the validation errors before saving."));
                }

                // Generate simple ID 
                var articleId = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                var article = new Article
                {
                    Id = articleId,
                    Title = title,
                    Category = category,
                    Author = string.IsNullOrWhiteSpace(author) ? "Unknown" : author,
                    Content = content
                };

                await _articleService.AddArticleAsync(article);

                return Ok(new ApiResult<object>(Status.Success, null, "Article added successfully"));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error adding article");
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while adding article"));
            }
        }

        /// <summary>
        /// Update existing article
        /// </summary>
        [HttpPut("UpdateArticle/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateArticle(string id, [FromBody] UpdateArticleRequest request)
        {
            try
            {
                // Check if article exists
                var existingArticle = await _articleService.GetArticleByIdAsync(id);
                if (existingArticle == null)
                {
                    return NotFound(new ApiResult<object>(Status.Error, null, "Article not found"));
                }

                var errors = new List<string>();
                var title = request.Title?.Trim();
                var category = request.CategoryId?.Trim();
                var content = request.Content?.Trim();
                var author = request.Author?.Trim();

                if (string.IsNullOrWhiteSpace(category))
                {
                    errors.Add("Please select a category.");
                }

                if (string.IsNullOrWhiteSpace(title))
                {
                    errors.Add("Title is required.");
                }
                else if (title.Length > 100)
                {
                    errors.Add("Title must not exceed 100 characters.");
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    errors.Add("Content is required.");
                }
                else if (content.Length < 50)
                {
                    errors.Add("Content must be at least 50 characters.");
                }

                if (errors.Any())
                {
                    return BadRequest(new ApiResult<object>(Status.Error, errors, "Please fix the validation errors before saving."));
                }

                // Update article properties
                existingArticle.Title = title;
                existingArticle.Category = category;
                existingArticle.Content = content;
                if (!string.IsNullOrWhiteSpace(author))
                {
                    existingArticle.Author = author;
                }

                await _articleService.UpdateArticleAsync(existingArticle);

                return Ok(new ApiResult<object>(Status.Success, null, "Article updated successfully"));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error updating article: {Id}", id);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while updating article"));
            }
        }

        /// <summary>
        /// Delete article
        /// </summary>
        [HttpDelete("DeleteArticle/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteArticle(string id)
        {
            try
            {
                // Check user role - only admin can delete
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(userRole) || (userRole.ToLower() != "admin" && userRole.ToLower() != "superadmin"))
                {
                    return Unauthorized(new ApiResult<object>(Status.Error, null, "Only administrators can delete articles"));
                }

                // Check if article exists
                var article = await _articleService.GetArticleByIdAsync(id);
                if (article == null)
                {
                    return NotFound(new ApiResult<object>(Status.Error, null, "Article not found"));
                }

                await _articleService.DeleteArticleAsync(id);

                return Ok(new ApiResult<object>(Status.Success, null, "Article deleted successfully"));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error deleting article: {Id}", id);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while deleting article"));
            }
        }

    }

    // Request model for adding article
    public class AddArticleRequest
    {
        public string Title { get; set; }
        public string CategoryId { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
    }

    // Request model for updating article
    public class UpdateArticleRequest
    {
        public string Title { get; set; }
        public string CategoryId { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
    }
}
