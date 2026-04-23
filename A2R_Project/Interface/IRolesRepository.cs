using A2R_Project.Models;

namespace A2R_Project.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllRoles();
        Task<string> Add(Role role);
        Task<bool> Edit(Role role);
        Task<Role?> GetById(int roleId);
        Task<bool> Delete(int roleId);
    }
}