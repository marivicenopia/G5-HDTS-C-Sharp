using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models;

public partial class Feedback
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Title { get; set; }

    public string Message { get; set; }

    public string Experience { get; set; }

    public DateTime? Date { get; set; }

    public string TicketId { get; set; }
}
