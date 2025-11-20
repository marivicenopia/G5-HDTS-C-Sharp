using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<List<Feedback>> GetAllFeedbacksAsync();
        Task<Feedback> GetFeedbackByIdAsync(string id);
        Task AddFeedbackAsync(Feedback feedback);
    }
}
