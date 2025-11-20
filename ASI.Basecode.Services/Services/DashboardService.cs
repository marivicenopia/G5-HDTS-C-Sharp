using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly NexDeskDbContext _context;

        public DashboardService(NexDeskDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDataViewModel> GetDashboardDataAsync(string userRole, string userId, string userDepartment = null)
        {
            var stats = await GetDashboardStatsAsync(userRole, userId, userDepartment);
            var ticketsByStatus = await GetTicketsByStatusAsync(userRole, userId, userDepartment);
            var ticketsByPriority = await GetTicketsByPriorityAsync(userRole, userId, userDepartment);
            var ticketTrends = await GetTicketTrendsAsync(userRole, userId, userDepartment);
            var recentTickets = await GetRecentTicketsAsync(userRole, userId, userDepartment);

            return new DashboardDataViewModel
            {
                Stats = stats,
                TicketsByStatus = ticketsByStatus,
                TicketsByPriority = ticketsByPriority,
                TicketTrends = ticketTrends,
                RecentTickets = recentTickets
            };
        }

        public async Task<DashboardStatsViewModel> GetDashboardStatsAsync(string userRole, string userId, string userDepartment = null)
        {
            var tickets = await GetFilteredTicketsQuery(userRole, userId, userDepartment).ToListAsync();
            var users = await GetFilteredUsersQuery(userRole, userDepartment).ToListAsync();

            var totalTickets = tickets.Count;
            var openTickets = tickets.Count(t => t.Status.Equals("Open", StringComparison.OrdinalIgnoreCase));
            var inProgressTickets = tickets.Count(t => t.Status.Equals("In Progress", StringComparison.OrdinalIgnoreCase));
            var resolvedTickets = tickets.Count(t => t.Status.Equals("Resolved", StringComparison.OrdinalIgnoreCase));
            var closedTickets = tickets.Count(t => t.Status.Equals("Closed", StringComparison.OrdinalIgnoreCase));

            var totalUsers = users.Count;
            var activeUsers = users.Count(u => u.IsActive);
            var totalAgents = users.Count(u => u.Role.Equals("Agent", StringComparison.OrdinalIgnoreCase) ||
                                           u.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase));

            // Calculate average resolution time (in hours)
            var resolvedTicketsWithDates = tickets
                .Where(t => t.Status.Equals("Resolved", StringComparison.OrdinalIgnoreCase) && t.ResolvedDate != DateTime.MinValue)
                .ToList();

            double averageResolutionTime = 0;
            if (resolvedTicketsWithDates.Count > 0)
            {
                var totalHours = resolvedTicketsWithDates
                    .Sum(t => (t.ResolvedDate - t.SubmittedDate).TotalHours);
                averageResolutionTime = totalHours / resolvedTicketsWithDates.Count;
            }

            // Calculate customer satisfaction (placeholder - could be from feedback table)
            var feedbacks = await _context.Feedbacks.Where(f => tickets.Select(t => t.Id).Contains(f.TicketId)).ToListAsync();
            double customerSatisfactionScore = feedbacks.Count > 0 ?
                feedbacks.Average(f => ConvertExperienceToScore(f.Experience)) : 4.2;

            return new DashboardStatsViewModel
            {
                TotalTickets = totalTickets,
                OpenTickets = openTickets,
                InProgressTickets = inProgressTickets,
                ResolvedTickets = resolvedTickets,
                ClosedTickets = closedTickets,
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                TotalAgents = totalAgents,
                AverageResolutionTime = Math.Round(averageResolutionTime, 1),
                CustomerSatisfactionScore = Math.Round(customerSatisfactionScore, 1)
            };
        }

        private async Task<List<TicketByStatusViewModel>> GetTicketsByStatusAsync(string userRole, string userId, string userDepartment = null)
        {
            var tickets = await GetFilteredTicketsQuery(userRole, userId, userDepartment).ToListAsync();

            var statusColors = new Dictionary<string, string>
            {
                { "Open", "#ef4444" },
                { "In Progress", "#f59e0b" },
                { "Resolved", "#10b981" },
                { "Closed", "#6b7280" }
            };

            var ticketsByStatus = tickets
                .GroupBy(t => t.Status)
                .Select(g => new TicketByStatusViewModel
                {
                    Status = g.Key,
                    Count = g.Count(),
                    Color = statusColors.GetValueOrDefault(g.Key, "#6b7280")
                })
                .OrderBy(x => x.Status)
                .ToList();

            return ticketsByStatus;
        }

        private async Task<List<TicketByPriorityViewModel>> GetTicketsByPriorityAsync(string userRole, string userId, string userDepartment = null)
        {
            var tickets = await GetFilteredTicketsQuery(userRole, userId, userDepartment).ToListAsync();

            var priorityColors = new Dictionary<string, string>
            {
                { "Low", "#10b981" },
                { "Medium", "#f59e0b" },
                { "High", "#ef4444" },
                { "Critical", "#dc2626" }
            };

            var ticketsByPriority = tickets
                .GroupBy(t => t.Priority)
                .Select(g => new TicketByPriorityViewModel
                {
                    Priority = g.Key,
                    Count = g.Count(),
                    Color = priorityColors.GetValueOrDefault(g.Key, "#6b7280")
                })
                .OrderBy(x => x.Priority)
                .ToList();

            return ticketsByPriority;
        }

        private async Task<List<TicketTrendViewModel>> GetTicketTrendsAsync(string userRole, string userId, string userDepartment = null)
        {
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var tickets = await GetFilteredTicketsQuery(userRole, userId, userDepartment)
                .Where(t => t.SubmittedDate >= thirtyDaysAgo)
                .ToListAsync();

            var ticketTrends = new List<TicketTrendViewModel>();

            for (int i = 29; i >= 0; i--)
            {
                var date = DateTime.Now.AddDays(-i);
                var dateString = date.ToString("MM/dd");

                var createdCount = tickets.Count(t => t.SubmittedDate.Date == date.Date);
                var resolvedCount = tickets.Count(t => t.ResolvedDate.Date == date.Date && t.ResolvedDate != DateTime.MinValue);

                ticketTrends.Add(new TicketTrendViewModel
                {
                    Date = dateString,
                    Created = createdCount,
                    Resolved = resolvedCount
                });
            }

            return ticketTrends;
        }

        private async Task<List<RecentTicketViewModel>> GetRecentTicketsAsync(string userRole, string userId, string userDepartment = null)
        {
            var recentTickets = await GetFilteredTicketsQuery(userRole, userId, userDepartment)
                .OrderByDescending(t => t.SubmittedDate)
                .Take(10)
                .Select(t => new RecentTicketViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Priority = t.Priority,
                    Status = t.Status,
                    SubmittedBy = t.SubmittedBy,
                    AssignedTo = t.AssignedTo,
                    SubmittedDate = t.SubmittedDate,
                    Department = t.Department
                })
                .ToListAsync();

            return recentTickets;
        }

        private IQueryable<Ticket> GetFilteredTicketsQuery(string userRole, string userId, string userDepartment = null)
        {
            var query = _context.Tickets.AsQueryable();

            switch (userRole?.ToLower())
            {
                case "superadmin":
                case "admin":
                    // Admins can see all tickets
                    break;
                case "agent":
                    // Agents can see tickets in their department or assigned to them
                    if (!string.IsNullOrEmpty(userDepartment))
                    {
                        query = query.Where(t => t.Department == userDepartment || t.AssignedTo == userId);
                    }
                    else
                    {
                        query = query.Where(t => t.AssignedTo == userId);
                    }
                    break;
                case "staff":
                case "user":
                default:
                    // Regular users can only see their own tickets
                    query = query.Where(t => t.SubmittedBy == userId);
                    break;
            }

            return query;
        }

        private IQueryable<User> GetFilteredUsersQuery(string userRole, string userDepartment = null)
        {
            var query = _context.Users.AsQueryable();

            switch (userRole?.ToLower())
            {
                case "superadmin":
                case "admin":
                    // Admins can see all users
                    break;
                case "agent":
                    // Agents can see users in their department
                    if (!string.IsNullOrEmpty(userDepartment))
                    {
                        query = query.Where(u => u.DepartmentId == userDepartment);
                    }
                    break;
                case "staff":
                case "user":
                default:
                    // Regular users can only see themselves
                    query = query.Where(u => false); // Return empty for user stats
                    break;
            }

            return query;
        }

        private double ConvertExperienceToScore(string experience)
        {
            // Convert experience text to numerical score (1-5)
            return experience?.ToLower() switch
            {
                "excellent" => 5.0,
                "very good" => 4.5,
                "good" => 4.0,
                "satisfactory" => 3.5,
                "poor" => 2.0,
                "very poor" => 1.0,
                _ => 3.0 // Default neutral score
            };
        }
    }
}