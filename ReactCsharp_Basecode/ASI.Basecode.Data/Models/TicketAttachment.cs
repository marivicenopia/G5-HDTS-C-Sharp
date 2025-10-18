using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models;

public partial class TicketAttachment
{
    public string Id { get; set; }

    public string TicketId { get; set; }

    public string Name { get; set; }

    public int? Size { get; set; }

    public string Type { get; set; }

    public DateTime? UploadDate { get; set; }

    public string Url { get; set; }
}
