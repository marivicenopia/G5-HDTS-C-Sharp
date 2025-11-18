using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IKnowledgeBaseService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
        Task<ArticleDetailDto> GetArticleAsync(string id);
        Task<ArticleDetailDto> AddArticleAsync(CreateArticleRequest request, string currentUser);
        Task DeleteArticleAsync(string id, string currentUserRole);
    }
}

