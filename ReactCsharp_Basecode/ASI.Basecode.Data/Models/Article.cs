using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models;

public partial class Article
{
    public string Id { get; set; }

    public string Title { get; set; }

    public string CategoryId { get; set; }

    public string Author { get; set; }

    public string Content { get; set; }

    public string Status { get; set; }

    public virtual Category Category { get; set; }
}
