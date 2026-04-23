namespace A2R_Project.Models
{
    public class DashboardStats
    {
      
        
            public int TotalStudents { get; set; }
            public int NewInquiries { get; set; }           // ✅ Keep INT
            public int Admissions { get; set; }             // ✅ Keep INT
            public decimal TotalRevenue { get; set; }       // ✅ Keep DECIMAL
        
    }

    public class RecentActivity
    {
        public string Icon { get; set; } = "";
        public string Title { get; set; } = "";
        public string TimeAgo { get; set; } = "";
        public string Type { get; set; } = "";
    }
}