using A2R_Project.Models;

namespace A2RSystemInterface
{
    public interface IAccessControl
    {
        Task<List<AccessControl>> GetAllAccessControls();
        Task<List<AccessControl>> GetFilteredAccessControls(string? roleName, string? userName);
        Task<string> UpdateAccessControl(List<AccessControl> accessControls);
        Task<string> InsertAccessControlValues(AccessControl request);
    }

   
}