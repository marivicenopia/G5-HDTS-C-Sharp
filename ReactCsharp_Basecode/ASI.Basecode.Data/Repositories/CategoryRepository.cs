using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Task<List<Category>> GetAllAsync()
        {
            return this.GetDbSet<Category>()
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public Task<Category> GetByIdAsync(string id)
        {
            return this.GetDbSet<Category>()
                .Include(c => c.Articles)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }
    }
}

