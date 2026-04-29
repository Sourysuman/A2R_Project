using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using A2R_Project.Services;
using Microsoft.Extensions.DependencyInjection;

namespace A2R_Project.Attributes
{
    public class PermissionAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _controller;
        private readonly string _action;

        public PermissionAttribute(string controller, string action)
        {
            _controller = controller;
            _action = action;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get PermissionService from DI
            var permissionService = context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();
            var username = context.HttpContext.Session.GetString("Username");

            // No login? Redirect
            if (string.IsNullOrEmpty(username))
            {
                context.Result = new RedirectToActionResult("Login", "AdminLogin", null);
                return;
            }

            // Check permission
            bool hasPermission = await permissionService.HasPermissionAsync(_controller, _action, username);

            if (!hasPermission)
            {
                // BLOCK ACCESS
                context.Result = new JsonResult(new { message = "❌ Access Denied - No Permission" })
                {
                    StatusCode = 403
                };
                return;
            }

            // ALLOW ACCESS
            await next();
        }
    }
}