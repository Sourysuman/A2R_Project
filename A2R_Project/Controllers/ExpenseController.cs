using A2R_Project.Interface;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace A2R_Project.Controllers
{
    public class ExpenseController : BaseController
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IExpenseCategoryRepository _expenseCategory;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ExpenseController(IExpenseRepository expenseRepository,
                               IExpenseCategoryRepository expenseCategory,
                               IWebHostEnvironment webHostEnvironment)
        {
            _expenseRepository = expenseRepository;
            _expenseCategory = expenseCategory;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            // ✅ Get all expenses and sort by latest first
            var allExpenses = (await _expenseRepository.GetAllExpenses())
                                .OrderByDescending(x => x.Expense_Date)
                                .ThenByDescending(x => x.Expense_ID) // for same date
                                .ToList();

            // ✅ Total count
            var totalCount = allExpenses.Count;

            // ✅ Pagination
            var pagedExpenses = allExpenses
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            // ✅ View Model
            var model = new ExpenseViewModel
            {
                expenses = pagedExpenses,
                expenseCategory = await _expenseCategory.GetAllExpenseCategories()
            };

            // ✅ ViewBag for pagination
            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.ShowAllButton = totalCount > pageSize;

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            var expenses = await _expenseRepository.GetAllExpenses();
            return Json(expenses);
        }

        [HttpPost]
        public async Task<IActionResult> Save(IFormFile? file, string Expense)
        {
            try
            {
                if (string.IsNullOrEmpty(Expense))
                    return Json("Error: Expense data is missing.");

                var expenseData = JsonConvert.DeserializeObject<Expense>(Expense);
                if (expenseData == null)
                    return Json("Error: Invalid expense data.");

                expenseData.Expense_ID = 0;
                var response = await _expenseRepository.AddExpense(expenseData, file);
                return Json(response);
            }
            catch (Exception ex)
            {
                return Json($"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetExpense(int expense_ID)
        {
            try
            {
                var expense = await _expenseRepository.GetExpenseById(expense_ID);
                if (expense == null)
                    return Json(new { success = false, message = "Expense not found" });

                // ✅ DYNAMIC IMAGE PATH - No hardcoded paths
                if (string.IsNullOrEmpty(expense.Expense_Proof))
                    expense.Expense_Proof = null;

                Console.WriteLine($"✅ GetExpense: {expense.Expense_Title}, Image: {expense.Expense_Proof}");
                return Json(expense);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ GetExpense Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
public async Task<IActionResult> Edit(IFormFile? file, string Expense)
{
    try
    {
        if (string.IsNullOrEmpty(Expense))
            return Json("Error: Expense data missing");

        var expenseData = JsonConvert.DeserializeObject<Expense>(Expense);
        if (expenseData?.Expense_ID <= 0)
            return Json("Error: Invalid expense ID");  // ✅ This was triggering

        Console.WriteLine($"🔧 EDIT ID: {expenseData.Expense_ID}");  // ✅ Debug
        
        var result = await _expenseRepository.EditExpense(expenseData, file, 
            expenseData.ExistingExpense_Proof ?? "");
        return Json(result);
    }
    catch (Exception ex)
    {
        return Json($"Error: {ex.Message}");
    }
}
        [HttpGet]
        public async Task<IActionResult> DeleteExpense(int expense_ID)
        {
            try
            {
                var result = await _expenseRepository.DeleteExpense(expense_ID);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json($"Error: {ex.Message}");
            }
        }
    }
}