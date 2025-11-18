using System.Collections.Generic;

namespace ASI.Basecode.Data.Models;

public partial class Category
{
    public string CategoryId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new HashSet<Article>();
}

