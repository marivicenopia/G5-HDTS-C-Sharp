using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class ArticleRepository : BaseRepository, IArticleRepository
    {
        public ArticleRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            return await GetDbSet<Article>().ToListAsync();
        }

        public async Task<Article> GetArticleByIdAsync(string id)
        {
            return await GetDbSet<Article>().FirstOrDefaultAsync(a => a.Id == id);
        }

        public Task AddArticleAsync(Article article)
        {
            GetDbSet<Article>().Add(article);
            UnitOfWork.SaveChanges();
            return Task.CompletedTask;
        }

        public Task UpdateArticleAsync(Article article)
        {
            SetEntityState(article, Microsoft.EntityFrameworkCore.EntityState.Modified);
            UnitOfWork.SaveChanges();
            return Task.CompletedTask;
        }

        public async Task DeleteArticleAsync(string id)
        {
            var article = await GetArticleByIdAsync(id);
            if (article != null)
            {
                GetDbSet<Article>().Remove(article);
                UnitOfWork.SaveChanges();
            }
        }
    }
}
