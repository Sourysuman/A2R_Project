using A2R_Project.Context;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using A2R_Project.Repositories;
using A2RSystemInterface;
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

        public AccessControlController(IAccessControl accessControl, ILoginRepository adminLogin, IRoleRepository role)
        {
            _accessControl = accessControl;
            _adminLogin = adminLogin;
            _role = role;
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
        public async Task<IActionResult> Edit([FromBody] List<AccessControl> accessControls)
        {
            try
            {
                var result = await _accessControl.UpdateAccessControl(accessControls);
                return Json(result.Contains("success") || result.Contains("modified") ? "success" : "failed");
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message);
            }
        }
    }
}