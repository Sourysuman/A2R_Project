using A2R_Project.Interfaces;
using A2R_Project.Models;
using A2R_Project.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace A2R_Project.Controllers
{
    public class ExpenseReportController : BaseController
    {
        private readonly IExpenseReportRepository _expenseReportRepository;

        public ExpenseReportController(IExpenseReportRepository expenseReportRepository)
        {
            _expenseReportRepository = expenseReportRepository;
        }

        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            try
            {
                List<ExpenseReport> allExpenseReports = await _expenseReportRepository.GetAllExpenseReport();
                int totalCount = allExpenseReports.Count;
                List<ExpenseReport> pagedExpenseReports = allExpenseReports
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var expenseCategories = pagedExpenseReports
                    .Select(e => new CategoryViewModel { Id = e.Category_ID, Name = e.Category_Name })
                    .DistinctBy(c => c.Id)
                    .ToList();

                ViewBag.TotalCount = totalCount;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.ExpenseCategories = expenseCategories;
                ViewBag.ShowAllButton = totalCount > pageSize;
                return View(pagedExpenseReports);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            try
            {
                var expenseReports = await _expenseReportRepository.GetAllExpenseReport();
                return Json(expenseReports);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Filter(string expenseType, string category, DateTime? fromDate, DateTime? toDate, string invoiceId)
        {
            try
            {
                var filteredExpenseReports = await _expenseReportRepository.GetFilteredExpenseReport(expenseType, category, fromDate, toDate, invoiceId);
                return Json(filteredExpenseReports);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}