using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController: ControllerBase<FeedbackController>
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IArticleService _articleService;

        public FeedbackController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IFeedbackService feedbackService,
            IArticleService articleService) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _feedbackService = feedbackService;
            _articleService = articleService;
        }

        /// <summary>
        /// Get all feedbacks
        /// </summary>
        [HttpGet("GetFeedbacks")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeedbacks()
        {
            try
            {
                var feedbacks = await _feedbackService.GetAllFeedbacksAsync();

                var feedbackList = feedbacks.Select(f => new
                {
                    id = f.Id,
                    name = f.Name,
                    email = f.Email,
                    title = f.Title,
                    rating = f.Experience,
                    feedbackText = f.Message,
                    date = f.Date
                }).ToList();

                return Ok(new ApiResult<object>(Status.Success, feedbackList, "Feedbacks retrieved successfully"));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error retrieving feedbacks");
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while retrieving feedbacks"));
            }
        }

        /// <summary>
        /// Get feedback by ID
        /// </summary>
        [HttpGet("GetFeedback/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeedback(string id)
        {
            try
            {
                var feedback = await _feedbackService.GetFeedbackByIdAsync(id);

                if (feedback == null)
                {
                    return NotFound(new ApiResult<object>(Status.Error, null, "Feedback not found"));
                }

                var response = new
                {
                    id = feedback.Id,
                    name = feedback.Name,
                    email = feedback.Email,
                    title = feedback.Title,
                    rating = feedback.Experience,
                    feedbackText = feedback.Message,
                    date = feedback.Date
                };

                return Ok(new ApiResult<object>(Status.Success, response, "Feedback retrieved successfully"));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error retrieving feedback: {Id}", id);
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while retrieving feedback"));
            }
        }

        /// <summary>
        /// Submit feedback
        /// </summary>
        [HttpPost("SubmitFeedback")]
        [AllowAnonymous]
        public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackRequest request)
        {
            try
            {
                var errors = new List<string>();
                var customerName = request.CustomerName?.Trim();
                var email = request.Email?.Trim();
                var rating = request.Rating?.Trim();
                var articleTitle = request.ArticleTitle?.Trim();
                var feedbackText = request.FeedbackText?.Trim();

                // Validate Customer Name (required)
                if (string.IsNullOrWhiteSpace(customerName))
                {
                    errors.Add("Customer Name is required.");
                }

                // Validate Email (required, format check)
                if (string.IsNullOrWhiteSpace(email))
                {
                    errors.Add("Email is required.");
                }
                else if (!IsValidEmail(email))
                {
                    errors.Add("Please enter a valid email address.");
                }

                // Validate Rating (required)
                if (string.IsNullOrWhiteSpace(rating))
                {
                    errors.Add("Experience Rating is required.");
                }
                else
                {
                    var validRatings = new[] { "Poor", "Fair", "Good", "Very Good", "Excellent" };
                    if (!validRatings.Contains(rating))
                    {
                        errors.Add("Please select a valid experience rating.");
                    }
                }

                // Validate Feedback Text (required, min 20 characters)
                if (string.IsNullOrWhiteSpace(feedbackText))
                {
                    errors.Add("Feedback text is required.");
                }
                else if (feedbackText.Length < 20)
                {
                    errors.Add("Feedback text must be at least 20 characters.");
                }

                if (errors.Any())
                {
                    return BadRequest(new ApiResult<object>(Status.Error, errors, "Please fix the validation errors before submitting."));
                }

                // Generate feedback ID
                var feedbackId = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                // Optional: Try to find article by title and link ArticleId
                string articleId = null;
                if (!string.IsNullOrWhiteSpace(articleTitle))
                {
                    var articles = await _articleService.GetAllArticlesAsync();
                    var matchingArticle = articles.FirstOrDefault(a => 
                        a.Title.Equals(articleTitle, StringComparison.OrdinalIgnoreCase));
                    if (matchingArticle != null)
                    {
                        articleId = matchingArticle.Id;
                    }
                }

                // Create feedback record
                var feedback = new Feedback
                {
                    Id = feedbackId,
                    Name = customerName,
                    Email = email,
                    Experience = rating,
                    Title = articleTitle,
                    Message = feedbackText,
                    Date = DateTime.Now,
                    TicketId = articleId ?? null // Store ArticleId in TicketId field if found
                };

                await _feedbackService.AddFeedbackAsync(feedback);

                return Ok(new ApiResult<object>(Status.Success, null, "Thank you for your feedback!"));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error submitting feedback");
                return StatusCode(500, new ApiResult<object>(Status.Error, null, "An error occurred while submitting feedback"));
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }

    // Request model for submitting feedback
    public class SubmitFeedbackRequest
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Rating { get; set; }
        public string ArticleTitle { get; set; }
        public string FeedbackText { get; set; }
    }
}
