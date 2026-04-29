using A2R_Project.Context;
using A2R_Project.Models;
using A2R_Project.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using System.Data;

namespace A2R_Project.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public PermissionService(AppDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        private async Task<string> GetUserRoleAsync(string username)
        {
            var cacheKey = $"role_{username}";
            if (_cache.TryGetValue(cacheKey, out string role)) return role ?? "";

            using var connection = _dbContext.CreateConnection();
            role = await connection.QueryFirstOrDefaultAsync<string>(
                "SELECT Role_Name FROM Logins WHERE UserName = @Username",
                new { Username = username });

            _cache.Set(cacheKey, role ?? "", TimeSpan.FromMinutes(30));
            return role ?? "";
        }

        public async Task<bool> HasPermissionAsync(string controller, string action, string username)
        {
            var cacheKey = $"perm_{username}_{controller}_{action}";
            if (_cache.TryGetValue(cacheKey, out bool hasPermission))
                return hasPermission;

            var roleName = await GetUserRoleAsync(username);
            if (string.IsNullOrEmpty(roleName)) return false;

            using var connection = _dbContext.CreateConnection();
            var count = await connection.QueryFirstOrDefaultAsync<int>(
                @"SELECT COUNT(*) FROM AccessControls 
                  WHERE RoleName = @RoleName 
                  AND Module_Flag = 1 
                  AND (@Controller IS NULL OR SubMenuController = @Controller)
                  AND (@Action IS NULL OR SubMenuAction = @Action)",
                new { RoleName = roleName, Controller = controller, Action = action });

            hasPermission = count > 0;
            _cache.Set(cacheKey, hasPermission, TimeSpan.FromMinutes(30));
            return hasPermission;
        }

        public async Task<List<SidebarMenu>> GetDynamicSidebarAsync(string username)
        {
            var cacheKey = $"sidebar_{username}";
            if (_cache.TryGetValue(cacheKey, out List<SidebarMenu> sidebar))
                return sidebar ?? new();

            var roleName = await GetUserRoleAsync(username);
            if (string.IsNullOrEmpty(roleName))
            {
                _cache.Set(cacheKey, new List<SidebarMenu>(), TimeSpan.FromMinutes(30));
                return new List<SidebarMenu>();
            }

            using var connection = _dbContext.CreateConnection();
            var permissions = await connection.QueryAsync<dynamic>(
                "sp_FilterAccessControls",
                new { RoleName = roleName, UserName = (string)null },
                commandType: CommandType.StoredProcedure);

            sidebar = permissions
                .GroupBy(p => new { p.MenuName, p.MenuIcon })
                .Select(g => new SidebarMenu
                {
                    MenuName = g.Key.MenuName ?? "Menu",
                    Icon = g.Key.MenuIcon ?? "layer-group",
                    SubMenus = g.Select(p => new SubMenuItem
                    {
                        Name = p.SubMenuName ?? "",
                        Controller = p.SubMenuController ?? "",
                        Action = p.SubMenuAction ?? "List"
                    }).Where(s => !string.IsNullOrEmpty(s.Controller)).ToList()
                })
                .Where(m => m.SubMenus.Any())
                .ToList();

            _cache.Set(cacheKey, sidebar, TimeSpan.FromMinutes(30));
            return sidebar;
        }
    }
}