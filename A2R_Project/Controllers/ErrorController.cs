using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;

public class ErrorController : Controller
{
    [Route("Error")]
    public IActionResult Index()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionFeature != null)
        {
            var ex = exceptionFeature.Error;

            // 🔥 Handle SQL Exception
            if (ex is SqlException)
            {
                ViewBag.ErrorTitle = "Database Error";
                ViewBag.ErrorMessage = "Database is not responding. Please try again later.";
            }
            // 🔥 Handle Timeout Exception
            else if (ex is TimeoutException)
            {
                ViewBag.ErrorTitle = "Timeout Error";
                ViewBag.ErrorMessage = "The request took too long. Please try again.";
            }
            // 🔥 Handle General Exception
            else
            {
                ViewBag.ErrorTitle = "Unexpected Error";
                ViewBag.ErrorMessage = "Something went wrong. Please try again.";
            }

            // Optional: store path
            ViewBag.ErrorPath = exceptionFeature.Path;
        }

        return View("~/Views/Shared/Error.cshtml");
    }
}