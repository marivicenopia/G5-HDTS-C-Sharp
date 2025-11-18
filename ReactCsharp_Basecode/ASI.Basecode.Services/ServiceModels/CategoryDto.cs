using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class CategoryDto
    {
        public string CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public IEnumerable<ArticleSummaryDto> Articles { get; set; }
    }
}

