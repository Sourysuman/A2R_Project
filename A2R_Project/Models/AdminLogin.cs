namespace A2R_Project.Models
{
    public class AdminLogin
    {
        public int LoginID { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Notes { get; set; }
        public int? Role_ID { get; set; }
        public string? Role_Name { get; set; }
        public int? IsActive { get; set; }
        public int? IsDeleted { get; set; }
        public string? Email { get; set; }
    }

    public class ResetPassword
    {
        public string Username { get; set; } = string.Empty;  
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class Logins
    {
        public List<AdminLogin> adminLogin { get; set; } = new List<AdminLogin>();
        public List<Role> role { get; set; } = new List<Role>();
    }
}