using A2R_Project.Interfaces;
using A2R_Project.Models;
using A2R_Project.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace A2R_Project.Controllers
{
    public class UserManagementController : BaseController
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IRoleRepository _roleRepository;

        public UserManagementController(ILoginRepository loginRepository, IRoleRepository roleRepository)
        {
            _loginRepository = loginRepository;
            _roleRepository = roleRepository;
        }

        // 🔥 MAIN USERS LIST PAGE (Same as your List action)
        [HttpGet]
        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            var models = new Logins();
            var allAdminLogins = await _loginRepository.GetAllAdminLogin();
            var totalCount = allAdminLogins.Count;
            var pagedAdminLogins = allAdminLogins.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            models.adminLogin = pagedAdminLogins;
            models.role = await _roleRepository.GetAllRoles();

            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Roles = models.role;
            return View(models);
        }

        // 🔥 ADD USER
        [HttpPost]
        public async Task<JsonResult> Save(AdminLogin loginData)
        {
            try
            {
                if (loginData == null || string.IsNullOrEmpty(loginData.UserName))
                {
                    return Json(new { success = false, message = "Invalid data" });
                }

                var response = await _loginRepository.Add(loginData);
                return Json(new { success = response.Contains("success"), message = response });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error: " + ex.Message });
            }
        }

        // 🔥 EDIT USER
        [HttpPost]
        public async Task<JsonResult> Edit(AdminLogin loginData)
        {
            try
            {
                if (loginData?.LoginID <= 0)
                {
                    return Json(new { success = false, message = "Invalid LoginID" });
                }

                Console.WriteLine($"Edit called: ID={loginData.LoginID}, Name={loginData.UserName}");
                var success = await _loginRepository.Edit(loginData);
                return Json(new { success = success, message = success ? "Updated successfully" : "Update failed" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Edit ERROR: {ex.Message}");
                return Json(new { success = false, message = $"Server error: {ex.Message}" });
            }
        }

        // 🔥 GET USER BY ID (For Edit/View/Delete modals)
        [HttpGet]
        public async Task<JsonResult> GetUser(string loginID)
        {
            try
            {
                if (!int.TryParse(loginID, out int id) || id <= 0)
                {
                    return Json(new { success = false, error = "Invalid ID format" });
                }

                var response = await _loginRepository.GetById(id);
                if (response != null)
                {
                    return Json(new { success = true, data = response });
                }
                return Json(new { success = false, error = "User not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Server error: " + ex.Message });
            }
        }

        // 🔥 DELETE USER
        [HttpGet]
        public async Task<JsonResult> DeleteUser(string loginID)
        {
            if (int.TryParse(loginID, out int id))
            {
                var success = await _loginRepository.Delete(id);
                return Json(new { success = success, message = success ? "Deleted successfully" : "Delete failed" });
            }
            return Json(new { success = false, message = "Invalid ID" });
        }

        // 🔥 GET ALL ROLES (For dropdowns)
        [HttpGet]
        public async Task<JsonResult> GetRoles()
        {
            try
            {
                var roles = await _roleRepository.GetAllRoles();
                return Json(new { success = true, data = roles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}