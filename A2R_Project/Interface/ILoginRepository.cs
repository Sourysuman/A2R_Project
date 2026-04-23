using A2R_Project.Models;

namespace A2R_Project.Interfaces
{
    public interface ILoginRepository
    {
        Task<List<AdminLogin>> GetAllAdminLogin();
        Task<string> Add(AdminLogin adminLogin);
        Task<bool> Edit(AdminLogin adminLogin);
        Task<AdminLogin> GetById(int LoginID);
        Task<bool> Delete(int LoginID);
        Task<AdminLogin> GetLoginUser(string userName, string password);
        Task<List<AdminLogin>> GetUsersByRole(string roleName);
        Task<string> GeneratePasswordResetTokenAsync(string username);
  // ✅ 3 parameters

        Task<AdminLogin> FindByEmailAsync(string email);
        Task<AdminLogin> FindByUsernameAsync(string username);
        Task<bool> ResetPasswordAsync(string username, string token, string newPassword);
    }

}