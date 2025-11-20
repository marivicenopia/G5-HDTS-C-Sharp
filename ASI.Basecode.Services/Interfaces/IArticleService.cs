using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IArticleService
    {
        Task<List<Article>> GetAllArticlesAsync();
        Task<Article> GetArticleByIdAsync(string id);
        Task AddArticleAsync(Article article);
        Task UpdateArticleAsync(Article article);
        Task DeleteArticleAsync(string id);
    }
}
