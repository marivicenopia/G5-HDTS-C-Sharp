namespace ASI.Basecode.Services.ServiceModels
{
    public class ArticleDetailDto : ArticleSummaryDto
    {
        public string Content { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}

