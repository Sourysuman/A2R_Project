using A2R_Project.Context;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using A2R_Project.Repositories;
using A2RSystemInterface;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A2R_Project.Controllers
{
    public class AccessControlController : BaseController
    {
        private readonly IAccessControl _accessControl;
        private readonly ILoginRepository _adminLogin;
        private readonly IRoleRepository _role;
        private readonly AppDbContext _dbContext;

        public AccessControlController(IAccessControl accessControl, ILoginRepository adminLogin, IRoleRepository role, AppDbContext dbContext  )
        {
            _accessControl = accessControl;
            _adminLogin = adminLogin;
            _role = role;
            _dbContext = dbContext;
        }

        // ✅ FIXED: List Action - Proper Role Population
        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            try
            {
                var accessControls = new AccessControls();

                // ✅ USE YOUR SAME SP for ALL data
                var allData = await _accessControl.GetAllAccessControls();
                var totalCount = allData.Count;
                var pagedData = allData.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                accessControls.accessControl = pagedData;

                // Roles
                var roles = await _role.GetAllRoles();
                ViewBag.Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Role_Name,  // ✅ Match your Roles table
                    Text = r.Role_Name
                }).ToList();

                ViewBag.TotalCount = totalCount;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;

                return View(accessControls);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"List Error: {ex.Message}");
                return View(new AccessControls());
            }
        }

        // ✅ FIXED: GetUsersByRole - Returns Proper JSON
        [HttpGet]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            try
            {
                if (string.IsNullOrEmpty(roleName))
                {
                    return Json(new List<SelectListItem>());
                }

                // ✅ YOUR EXISTING METHOD
                var users = await _adminLogin.GetUsersByRole(roleName);

                var userList = users.Select(u => new SelectListItem
                {
                    Value = u.LoginID.ToString(),
                    Text = u.UserName
                }).ToList();

                return Json(userList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetUsersByRole Error: {ex.Message}");
                return Json(new List<SelectListItem>());
            }
        }
        [HttpPost]
        public async Task<IActionResult> Filter(string roleName, string userName)
        {
            try
            {
                Console.WriteLine($"🔍 Filter: Role='{roleName}', User='{userName}'");

                var data = await _accessControl.GetFilteredAccessControls(roleName, userName);
                Console.WriteLine($"✅ Filter Result: {data.Count} records");

                return Json(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Filter Error: {ex.Message}");
                return Json(new List<AccessControl>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] AccessControl request)
        {
            try
            {
                var result = await _accessControl.InsertAccessControlValues(request);
                return Json(result.Contains("success") ? "success" : "failed");
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message);
            }
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] List<AccessControl> accessControls)
        {
            try
            {
                var result = await _accessControl.UpdateAccessControl(accessControls);
                if (result.Contains("Updated"))
                    return Json("success");
                return Json("failed");
            }
            catch (Exception ex)
            {
                return Json("failed");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDynamicSidebar(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                    return Json(new { menus = new List<object>() });

                int userId = await GetUserId(username);
                if (userId == 0) return Json(new { menus = new List<object>() });

                using var connection = _dbContext.CreateConnection();

                var permissions = await connection.QueryAsync(@"
            SELECT ac.AccessID, ac.Submenu_ID, ac.Module_Flag,
                   sm.SubMenuName, sm.Controller, sm.Action, sm.IconClass,
                   sm.Menu_ID
            FROM AccessControl ac
            JOIN SubMenu sm ON ac.Submenu_ID = sm.SubMenu_ID
            WHERE ac.User_ID = @UserId AND ac.Module_Flag = 1
            ORDER BY sm.Menu_ID, sm.SubMenuName",
                    new { UserId = userId });

                // 🔥 Group by Menu_ID
                var sidebarMenus = permissions
                    .GroupBy(p => p.Menu_ID)
                    .Select(g => new {
                        menuName = GetMenuName(g.Key),
                        icon = GetIconClass(g.Key.ToString()),  // ✅ Fixed
                        subMenus = g.Select(p => new {
                            name = p.SubMenuName,
                            controller = p.Controller ?? "Home",
                            action = p.Action ?? "Index"
                        }).ToList()
                    })
                    .Where(m => m.subMenus.Any())
                    .ToList();

                return Json(new { menus = sidebarMenus });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sidebar Error: {ex.Message}");
                return Json(new { menus = new List<object>() });
            }
        }

        // 🔥 FIXED Icon method
        private string GetIconClass(string menuIdOrName)
        {
            return menuIdOrName switch
            {
                "1" => "tachometer-alt",      // Dashboard
                "2" => "building-columns",    // Organization
                "3" => "wallet",              // Account Management  
                "4" => "users",               // Student Management
                "5" => "money-bill-wave",     // Fees Collection
                "6" => "user-shield",         // Users
                "9" => "key",                 // Roles
                _ => "layer-group"
            };
        }

        private string GetMenuName(int? menuId)
        {
            return menuId switch
            {
                1 => "Dashboard",
                2 => "Organization",
                3 => "Account Management",
                4 => "Student Management",
                5 => "Fees Collection",
                6 => "Users",
                9 => "Roles",
                _ => "General"
            };
        }

        private async Task<int> GetUserId(string username)
        {
            using var connection = _dbContext.CreateConnection();
            // 🔥 FIXED: Admin_Login (not Logins)
            return await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT LoginID FROM Admin_Login WHERE UserName = @Username",
                new { Username = username });
        }
    }
}