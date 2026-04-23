using A2R_Project.Models;

namespace A2R_Project.Services
{
    public interface IDashboardService
    {
        Task<DashboardStats> GetDashboardStatsAsync();
        Task<List<RecentActivity>> GetRecentActivitiesAsync();
    }
}