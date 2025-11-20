using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class ArticleService: IArticleService    
    {
        private readonly IArticleRepository _articleRepository;
        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            return await _articleRepository.GetAllArticlesAsync();
        }

        public async Task<Article> GetArticleByIdAsync(string id)
        {
            return await _articleRepository.GetArticleByIdAsync(id);
        }

        public async Task AddArticleAsync(Article article)
        {
            await _articleRepository.AddArticleAsync(article);
        }

        public async Task UpdateArticleAsync(Article article)
        {
            await _articleRepository.UpdateArticleAsync(article);
        }

        public async Task DeleteArticleAsync(string id)
        {
            await _articleRepository.DeleteArticleAsync(id);
        }
    }
}
