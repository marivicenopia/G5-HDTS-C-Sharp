using System;
using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class DashboardStatsViewModel
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalAgents { get; set; }
        public double AverageResolutionTime { get; set; }
        public double CustomerSatisfactionScore { get; set; }
    }

    public class TicketByStatusViewModel
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public string Color { get; set; }
    }

    public class TicketByPriorityViewModel
    {
        public string Priority { get; set; }
        public int Count { get; set; }
        public string Color { get; set; }
    }

    public class TicketTrendViewModel
    {
        public string Date { get; set; }
        public int Created { get; set; }
        public int Resolved { get; set; }
    }

    public class RecentTicketViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string SubmittedBy { get; set; }
        public string AssignedTo { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string Department { get; set; }
    }

    public class DashboardDataViewModel
    {
        public DashboardStatsViewModel Stats { get; set; }
        public List<TicketByStatusViewModel> TicketsByStatus { get; set; }
        public List<TicketByPriorityViewModel> TicketsByPriority { get; set; }
        public List<TicketTrendViewModel> TicketTrends { get; set; }
        public List<RecentTicketViewModel> RecentTickets { get; set; }
    }

    public class RoleBasedDashboardViewModel
    {
        public string UserRole { get; set; }
        public string UserDepartment { get; set; }
        public DashboardDataViewModel Dashboard { get; set; }
    }
}