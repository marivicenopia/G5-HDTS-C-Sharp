using ASI.Basecode.Services.ServiceModels;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDataViewModel> GetDashboardDataAsync(string userRole, string userId, string userDepartment = null);
        Task<DashboardStatsViewModel> GetDashboardStatsAsync(string userRole, string userId, string userDepartment = null);
    }
}