using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using A2R_Project.Services;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using System.Data;
using A2R_Project.Context;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace A2R_Project.Controllers
{
    public class BaseController : Controller
    {
        protected IPermissionService _permissionService;
        protected AppDbContext _dbContext;

        public BaseController()
        {
            _permissionService = HttpContext?.RequestServices?.GetService<IPermissionService>();
            _dbContext = HttpContext?.RequestServices?.GetService<AppDbContext>();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            context.HttpContext.Response.Headers["Expires"] = "0";
            context.HttpContext.Response.Headers["Vary"] = "*";

            // 🔥 SAFE Session handling
            ViewBag.Username = HttpContext.Session.GetString("Username") ?? "Guest";
            ViewBag.LoginID = HttpContext.Session.GetString("LoginID") ?? "";
            ViewBag.UserPassword = HttpContext.Session.GetString("UserPassword") ?? "";

            // 🔥 NULL CHECK
            if (string.IsNullOrEmpty(ViewBag.Username as string) || (ViewBag.Username as string) == "Guest")
            {
                context.Result = new RedirectToActionResult("Login", "AdminLogin", null);
                return;
            }

            string controllerName = context.RouteData.Values["controller"]?.ToString() ?? "";
            string actionName = context.RouteData.Values["action"]?.ToString() ?? "";

            // 🔥 1. Module Flag check
            if (!CheckModuleFlag(controllerName))
            {
                TempData["Error"] = $"❌ No {controllerName} Module Access!";
                context.Result = new RedirectToActionResult("Index", "Dashboard", null);
                return;
            }

            // 🔥 2. Action Flag check  
            if (!CheckActionFlag(controllerName, actionName))
            {
                TempData["Error"] = $"❌ {actionName} Not Allowed!";
                context.Result = new RedirectToActionResult("Index", controllerName, null);
                return;
            }

            base.OnActionExecuting(context);
        }

        private bool CheckModuleFlag(string controller)
        {
            try
            {
                if (_dbContext == null) return true;

                string username = ViewBag.Username as string;
                if (string.IsNullOrEmpty(username) || username == "Admin") return true;

                using var conn = _dbContext.CreateConnection();
                if (conn.State != ConnectionState.Open) conn.Open();

                var userId = conn.QueryFirstOrDefault<int>(
                    "SELECT LoginID FROM Admin_Login WHERE UserName = @Username",
                    new { Username = username });

                if (userId == 0) return false;

                var cleanController = controller.TrimEnd('s').ToLower();
                var hasAccess = conn.QueryFirstOrDefault<int>(@"
                    SELECT COUNT(*) 
                    FROM AccessControl ac 
                    JOIN SubMenu sm ON ac.Submenu_ID = sm.SubMenu_ID
                    WHERE ac.User_ID = @UserId 
                    AND LOWER(REPLACE(sm.Controller, 's', '')) = @CleanController
                    AND ac.Module_Flag = 1",
                    new { UserId = userId, CleanController = cleanController }) > 0;

                return hasAccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ModuleFlag Error: {ex.Message}");
                return true; // Allow on error
            }
        }

        private bool CheckActionFlag(string controller, string action)
        {
            try
            {
                if (_dbContext == null) return true;

                string username = ViewBag.Username as string;
                if (string.IsNullOrEmpty(username)) return true;

                using var conn = _dbContext.CreateConnection();
                if (conn.State != ConnectionState.Open) conn.Open();

                var userId = conn.QueryFirstOrDefault<int>(
                    "SELECT LoginID FROM Admin_Login WHERE UserName = @Username",
                    new { Username = username });

                var cleanController = controller.TrimEnd('s').ToLower();

                // 🔥 Checkbox → Action PERFECT MAPPING
                string flagColumn = action.ToLower() switch
                {
                    "create" or "insert" => "Insert_Flag",
                    "edit" or "update" => "Update_Flag",
                    "delete" or "deletes" => "Delete_Flag",
                    _ => "Module_Flag"  // List/Index/Details
                };

                var hasPermission = conn.QueryFirstOrDefault<int>($@"
                    SELECT ISNULL({flagColumn}, 0)
                    FROM AccessControl ac 
                    JOIN SubMenu sm ON ac.Submenu_ID = sm.SubMenu_ID
                    WHERE ac.User_ID = @UserId 
                    AND LOWER(REPLACE(sm.Controller, 's', '')) = @CleanController
                    AND ac.Module_Flag = 1",
                    new { UserId = userId, CleanController = cleanController }) == 1;

                return hasPermission;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ActionFlag Error: {ex.Message}");
                return true;
            }
        }

        // Keep async methods unchanged
        public async Task<(bool CanInsert, bool CanUpdate, bool CanDelete)> GetCrudPermissionsAsync(string controller, string action)
        {
            var username = ViewBag.Username?.ToString();
            if (string.IsNullOrEmpty(username) || _dbContext == null) return (false, false, false);

            using var conn = _dbContext.CreateConnection();
            var userId = await conn.QueryFirstOrDefaultAsync<int>("SELECT LoginID FROM Admin_Login WHERE UserName = @Username", new { Username = username });
            if (userId == 0) return (false, false, false);

            var perm = await conn.QueryFirstOrDefaultAsync(@"
                SELECT Insert_Flag, Update_Flag, Delete_Flag
                FROM AccessControl ac JOIN SubMenu sm ON ac.Submenu_ID = sm.SubMenu_ID
                WHERE ac.User_ID = @UserId AND LOWER(REPLACE(sm.Controller,'s','')) = LOWER(REPLACE(@Controller,'s',''))
                AND LOWER(sm.Action) = LOWER(@Action) AND ac.Module_Flag = 1",
                new { UserId = userId, Controller = controller, Action = action });

            return (perm?.Insert_Flag == 1 ?? false, perm?.Update_Flag == 1 ?? false, perm?.Delete_Flag == 1 ?? false);
        }

        public async Task<bool> CanAccessAsync(string controller, string action)
        {
            if (_permissionService != null)
                return await _permissionService.HasPermissionAsync(controller, action, ViewBag.Username?.ToString() ?? "");
            var (canInsert, _, _) = await GetCrudPermissionsAsync(controller, action);
            return canInsert || ViewBag.Username?.ToString() == "Admin";
        }


        protected async Task SetPagePermissions(string controllerName)
        {
            // ✅ Get _dbContext from HttpContext here — never null
            _dbContext = HttpContext.RequestServices.GetService<AppDbContext>();

            string username = HttpContext.Session.GetString("Username") ?? "";

            if (string.IsNullOrEmpty(username))
            {
                ViewBag.CanInsert = false;
                ViewBag.CanUpdate = false;
                ViewBag.CanDelete = false;
                return;
            }

            using var conn = _dbContext.CreateConnection();

            int userId = await conn.QueryFirstOrDefaultAsync<int>(
                "SELECT LoginID FROM Admin_Login WHERE UserName = @Username",
                new { Username = username });

            if (userId == 0)
            {
                ViewBag.CanInsert = false;
                ViewBag.CanUpdate = false;
                ViewBag.CanDelete = false;
                return;
            }

            var flags = await conn.QueryFirstOrDefaultAsync(@"
        SELECT 
            ac.Insert_Flag, 
            ac.Update_Flag, 
            ac.Delete_Flag
        FROM AccessControl ac
        JOIN SubMenu sm ON ac.Submenu_ID = sm.SubMenu_ID
        WHERE ac.User_ID = @UserId
        AND LOWER(sm.Controller) = LOWER(@Controller)
        AND ac.Module_Flag = 1",
                new { UserId = userId, Controller = controllerName });

            ViewBag.CanInsert = flags?.Insert_Flag == 1;
            ViewBag.CanUpdate = flags?.Update_Flag == 1;
            ViewBag.CanDelete = flags?.Delete_Flag == 1;
        }









    }
}