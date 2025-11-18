using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class KnowledgeBaseService : IKnowledgeBaseService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public KnowledgeBaseService(
            IArticleRepository articleRepository,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var articles = await _articleRepository.Query()
                .Include(a => a.Category)
                .OrderBy(a => a.Title)
                .ToListAsync();

            return categories.Select(category =>
            {
                var categoryArticles = articles
                    .Where(a => a.CategoryId == category.CategoryId)
                    .Select(MapToSummaryDto)
                    .ToList();

                return new CategoryDto
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    Description = category.Description,
                    DisplayOrder = category.DisplayOrder,
                    Articles = categoryArticles
                };
            }).ToList();
        }

        public async Task<ArticleDetailDto> GetArticleAsync(string id)
        {
            var article = await _articleRepository.GetByIdAsync(id);
            return article == null ? null : MapToDetailDto(article);
        }

        public async Task<ArticleDetailDto> AddArticleAsync(CreateArticleRequest request, string currentUser)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Category not found.");
            }

            var article = new Article
            {
                Id = GenerateArticleId(),
                Title = request.Title,
                CategoryId = request.CategoryId,
                Author = string.IsNullOrWhiteSpace(request.Author) ? currentUser : request.Author,
                Content = request.Content,
                Status = "ACTIVE"
            };

            await _articleRepository.AddAsync(article);
            _unitOfWork.SaveChanges();

            article.Category = category;
            return MapToDetailDto(article);
        }

        public async Task DeleteArticleAsync(string id, string currentUserRole)
        {
            var article = await _articleRepository.GetByIdAsync(id);
            if (article == null)
            {
                throw new KeyNotFoundException("Article not found.");
            }

            if (!string.Equals(currentUserRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Only admins can delete articles.");
            }

            if (string.Equals(article.Status, "COMPLETED", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Completed articles cannot be deleted.");
            }

            await _articleRepository.RemoveAsync(article);
            _unitOfWork.SaveChanges();
        }

        private static ArticleSummaryDto MapToSummaryDto(Article article)
        {
            return new ArticleSummaryDto
            {
                Id = article.Id,
                Title = article.Title,
                Author = article.Author,
                Status = article.Status
            };
        }

        private static ArticleDetailDto MapToDetailDto(Article article)
        {
            return new ArticleDetailDto
            {
                Id = article.Id,
                Title = article.Title,
                Author = article.Author,
                Content = article.Content,
                Status = article.Status,
                CategoryId = article.CategoryId,
                CategoryName = article.Category?.Name
            };
        }

        private static string GenerateArticleId()
        {
            return Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
        }
    }
}

