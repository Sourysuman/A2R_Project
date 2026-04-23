using A2R_Project.Interfaces;
using A2R_Project.Models;
using A2R_Project.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace A2R_Project.Controllers
{
    public class AdminLoginController : Controller
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IRoleRepository _role;

        public AdminLoginController(ILoginRepository loginRepository, IRoleRepository _role)
        {
            _loginRepository = loginRepository;
            this._role = _role;
        }

        // LOGIN PAGE
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLogin adminLogin)
        {
            if (!string.IsNullOrEmpty(adminLogin.UserName) && !string.IsNullOrEmpty(adminLogin.Password))
            {
                var result = await _loginRepository.GetLoginUser(adminLogin.UserName, adminLogin.Password);

                if (result != null && result.LoginID > 0)
                {
                    HttpContext.Session.SetString("Username", result.UserName);
                    HttpContext.Session.SetString("LoginID", result.LoginID.ToString());
                    HttpContext.Session.SetString("UserRole", result.Role_ID.ToString());
                    HttpContext.Session.SetString("UserPassword", adminLogin.Password);


                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.errormessage = "Invalid Username and Password";
                }
            }
            return View(adminLogin);
        }

        // ADMIN LOGIN LIST & MANAGEMENT

        // ADMIN LOGIN LIST & MANAGEMENT
        [HttpGet]
        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            var models = new Logins();
            var allAdminLogins = await _loginRepository.GetAllAdminLogin();
            var totalCount = allAdminLogins.Count;
            var pagedAdminLogins = allAdminLogins.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            models.adminLogin = pagedAdminLogins;
            models.role = await _role.GetAllRoles(); // Fixed: use _roleRepository

            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Roles = models.role; // Fixed: Pass roles to ViewBag for modals
            return View(models);
        }

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

        [HttpPost]
        public async Task<JsonResult> Edit(AdminLogin loginData)
        {
            try
            {
                if (loginData?.LoginID <= 0)
                {
                    return Json(new { success = false, message = "Invalid LoginID" });
                }

                // ✅ DEBUG: Log incoming data
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

        [HttpGet]
        public async Task<JsonResult> GetAdminLogin(string loginID)
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

        [HttpGet]
        public async Task<JsonResult> DeleteAdminLogin(string loginID)
        {
            if (int.TryParse(loginID, out int id))
            {
                var success = await _loginRepository.Delete(id);
                return Json(new { success = success, message = success ? "Deleted successfully" : "Delete failed" });
            }
            return Json(new { success = false, message = "Invalid ID" });
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        // ... (keep all existing methods, add these forgot password methods)
        [HttpGet]
        public IActionResult GetUserCredentials()
        {
            return Json(new
            {
                success = true,
                username = HttpContext.Session.GetString("Username") ?? "",
                password = HttpContext.Session.GetString("UserPassword") ?? ""
            });
        }
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
            [HttpPost]
            public async Task<IActionResult> ForgotPassword(string username)
            {
                try
                {
                    if (string.IsNullOrEmpty(username?.Trim()))
                        return Json(new { success = false, message = "Username required" });

                    string cleanUsername = username.Trim();
                    Console.WriteLine($"🔍 Looking for username: '{cleanUsername}'");

                    var user = await _loginRepository.FindByUsernameAsync(cleanUsername);
                    if (user == null)
                    {
                        Console.WriteLine("❌ User not found");
                        return Json(new { success = false, message = "Username not found" });
                    }

                    Console.WriteLine("✅ User found, generating token...");
                    var token = await _loginRepository.GeneratePasswordResetTokenAsync(cleanUsername);

                    if (string.IsNullOrEmpty(token))
                    {
                        Console.WriteLine("❌ Failed to generate token");
                        return Json(new { success = false, message = "Failed to generate token" });
                    }

                    TempData["ResetUsername"] = cleanUsername;
                    TempData["ResetToken"] = token;
                    Console.WriteLine($"✅ Token generated: {token.Substring(0, 8)}...");

                    return Json(new { success = true, message = "Enter new password below" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"💥 ForgotPassword ERROR: {ex}");
                    return Json(new { success = false, message = $"Server error: {ex.Message}" });
                }
            }
            [HttpPost]
        public async Task<IActionResult> ResetPassword()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                Console.WriteLine($"🔥 ResetPassword body: {body}");

                dynamic model = JsonConvert.DeserializeObject(body);
                string newPassword = model?.NewPassword?.ToString() ?? "";
                string confirmPassword = model?.ConfirmPassword?.ToString() ?? "";

                Console.WriteLine($"Passwords: new={newPassword?.Length} chars, match={newPassword == confirmPassword}");

                if (newPassword != confirmPassword || string.IsNullOrEmpty(newPassword))
                    return Json(new { success = false, message = "Passwords don't match or empty" });

                string username = TempData["ResetUsername"]?.ToString() ?? "";
                string token = TempData["ResetToken"]?.ToString() ?? "";

                Console.WriteLine($"TempData - Username: {username}, Token: {token?.Substring(0, 8)}...");

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
                    return Json(new { success = false, message = "Session expired. Generate token again." });

                bool result = await _loginRepository.ResetPasswordAsync(username, token, newPassword);

                if (result)
                {
                    TempData.Remove("ResetUsername");
                    TempData.Remove("ResetToken");
                    Console.WriteLine("✅ Password reset SUCCESS");
                }

                return Json(new { success = result, message = result ? "✅ Password reset success!" : "❌ Invalid/expired token" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ResetPassword ERROR: {ex}");
                return Json(new { success = false, message = $"Server error: {ex.Message}" });
            }
        }
    }
    }