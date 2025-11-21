using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<List<Feedback>> GetAllFeedbacksAsync()
        {
            return await _feedbackRepository.GetAllFeedbacksAsync();
        }

        public async Task<Feedback> GetFeedbackByIdAsync(string id)
        {
            return await _feedbackRepository.GetFeedbackByIdAsync(id);
        }

        public async Task AddFeedbackAsync(Feedback feedback)
        {
            await _feedbackRepository.AddFeedbackAsync(feedback);
        }
    }
}
