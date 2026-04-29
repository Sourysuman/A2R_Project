using A2R_Project.Models;
using System.Threading.Tasks;

namespace A2R_Project.Services
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(string controller, string action, string username);
        Task<List<SidebarMenu>> GetDynamicSidebarAsync(string username);
    }
}