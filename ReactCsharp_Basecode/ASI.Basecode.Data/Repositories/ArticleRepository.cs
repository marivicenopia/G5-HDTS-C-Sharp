using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class ArticleRepository : BaseRepository, IArticleRepository
    {
        public ArticleRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Article> Query()
        {
            return this.GetDbSet<Article>();
        }

        public Task<Article> GetByIdAsync(string id)
        {
            return this.GetDbSet<Article>()
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Article article)
        {
            await this.GetDbSet<Article>().AddAsync(article);
        }

        public Task RemoveAsync(Article article)
        {
            this.GetDbSet<Article>().Remove(article);
            return Task.CompletedTask;
        }
    }
}

