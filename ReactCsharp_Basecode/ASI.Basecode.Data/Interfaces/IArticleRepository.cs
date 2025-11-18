using ASI.Basecode.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IArticleRepository
    {
        IQueryable<Article> Query();
        Task<Article> GetByIdAsync(string id);
        Task AddAsync(Article article);
        Task RemoveAsync(Article article);
    }
}

