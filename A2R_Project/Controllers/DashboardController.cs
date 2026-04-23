using Microsoft.AspNetCore.Mvc;
using A2R_Project.Services;
using A2R_Project.Models;

namespace A2R_Project.Controllers
{
    
    public class DashboardController : BaseController  
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

     
        public async Task<IActionResult> Index()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            var activities = await _dashboardService.GetRecentActivitiesAsync();

            ViewBag.Stats = stats;
            ViewBag.Activities = activities;
            

            return View();
        }

     
        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();
                var activities = await _dashboardService.GetRecentActivitiesAsync();

                return Json(new
                {
                    stats = stats ?? new DashboardStats(),
                    activities = activities ?? new List<RecentActivity>()
                });
            }
            catch
            {
                return Json(new
                {
                    stats = new DashboardStats(),
                    activities = new List<RecentActivity>()
                });
            }
        }
    }
}