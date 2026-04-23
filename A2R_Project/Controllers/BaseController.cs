using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters; 

namespace A2R_Project.Controllers
{
    public class BaseController : Controller
    {
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            ViewBag.Username = HttpContext.Session.GetString("Username") ?? "Guest";
            ViewBag.LoginID = HttpContext.Session.GetString("LoginID") ?? "";
            ViewBag.UserPassword = HttpContext.Session.GetString("UserPassword") ?? "";


            if (string.IsNullOrEmpty(ViewBag.Username) || ViewBag.Username == "Guest")
            {
                context.Result = new RedirectToActionResult("Login", "AdminLogin", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}