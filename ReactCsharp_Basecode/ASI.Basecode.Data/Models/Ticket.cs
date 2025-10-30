using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models;

public partial class Ticket
{
    public string Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Priority { get; set; }

    public string Department { get; set; }

    public string SubmittedBy { get; set; }

    public DateTime SubmittedDate { get; set; }

    public string Status { get; set; }

    public string AssignedTo { get; set; }

    public string ResolvedBy { get; set; }

    public DateTime? ResolvedDate { get; set; }


    public string ResolutionDescription { get; set; }

    public string AgentFeedback { get; set; }
}
