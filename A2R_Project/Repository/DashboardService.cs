using Dapper;
using A2R_Project.Context;
using A2R_Project.Models;

namespace A2R_Project.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _dbContext;

        public DashboardService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            using var connection = _dbContext.CreateConnection();

            return new DashboardStats
            {
                TotalStudents = await connection.QuerySingleAsync<int>(
                    "SELECT COUNT(*) FROM StudentInquiry WHERE IsActive = 1"),

                NewInquiries = await connection.QuerySingleAsync<int>(
                    "SELECT COUNT(*) FROM StudentInquiry WHERE IsActive = 1"),

                Admissions = await connection.QuerySingleAsync<int>(
                    "SELECT COUNT(*) FROM Admission WHERE IsActive = 1"),

                TotalRevenue = await connection.QuerySingleAsync<decimal?>(
                    "SELECT ISNULL(SUM(Net_Amount), 0) FROM Admission WHERE IsActive = 1") ?? 0M
            };
        }

        public async Task<List<RecentActivity>> GetRecentActivitiesAsync()
        {
            using var connection = _dbContext.CreateConnection();

            // ✅ SEPARATE QUERIES - No UNION problems
            var inquiries = await connection.QueryAsync<RecentActivity>(@"
                SELECT TOP 5 
                    'fa-clipboard-question' as Icon,
                    'Inquiry: ' + ISNULL(NameOfStudent,'N/A') + ' | ' + ISNULL(Email,'No Email') as Title,
                    CASE 
                        WHEN Created_Date IS NULL THEN 'Inquiry #' + CAST(StudentInquiryID AS VARCHAR(10))
                        ELSE FORMAT(Created_Date, 'MM-dd HH:mm')
                    END as TimeAgo,
                    'primary' as Type
                FROM StudentInquiry 
                WHERE IsActive = 1 
                ORDER BY StudentInquiryID DESC");

            var admissions = await connection.QueryAsync<RecentActivity>(@"
                SELECT TOP 5 
                    'fa-user-plus' as Icon,
                    'Admission #' + CAST(AdmissionID AS VARCHAR(10)) + ' | ₹' + CAST(ISNULL(Net_Amount,0) AS VARCHAR(20)) as Title,
                    ISNULL(FORMAT(Created_Date, 'MM-dd HH:mm'), 'Recent') as TimeAgo,
                    'success' as Type
                FROM Admission 
                WHERE IsActive = 1 
                ORDER BY Created_Date DESC, AdmissionID DESC");

            // ✅ Combine results
            var activities = inquiries.Concat(admissions).Take(10).ToList();
            return activities.ToList();
        }
    }
}