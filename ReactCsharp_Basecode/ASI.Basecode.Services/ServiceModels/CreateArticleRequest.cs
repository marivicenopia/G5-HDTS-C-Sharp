using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class CreateArticleRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(20)]
        public string CategoryId { get; set; }

        [MaxLength(100)]
        public string Author { get; set; }

        [Required]
        public string Content { get; set; }
    }
}

