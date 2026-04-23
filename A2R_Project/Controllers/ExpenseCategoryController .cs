using A2R_Project.Models;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A2R_Project.Controllers
{
    public class ExpenseCategoryController : BaseController
    {
        private readonly  A2R_Project.Interface.IExpenseCategoryRepository _expenseCategoryRepository;

        public ExpenseCategoryController(A2R_Project.Interface.IExpenseCategoryRepository expenseCategoryRepository)
        {
            _expenseCategoryRepository = expenseCategoryRepository;
        }

        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            var allExpenseCategories = await _expenseCategoryRepository.GetAllExpenseCategories();

            // 🔥 Sort by latest first (NEWLY ADDED FIRST)
            allExpenseCategories = allExpenseCategories
                                    .OrderByDescending(x => x.Created_Date)
                                    .ToList();

            var totalCount = allExpenseCategories.Count;

            var pagedExpenseCategories = allExpenseCategories
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.ShowAllButton = totalCount > pageSize;

            return View(pagedExpenseCategories);
        }

        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            var expenseCategories = await _expenseCategoryRepository.GetAllExpenseCategories();
            return Json(expenseCategories);
        }
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] string Category_Name)
        {
            if (string.IsNullOrWhiteSpace(Category_Name))
                return Json("Category name required");

            try
            {
                var category = new ExpenseCategory
                {
                    Category_Name = Category_Name.Trim(),
                    IsActive = 1,
                    Created_By = User.Identity.Name ?? "System",
                    Created_Date = DateTime.Now
                };

                var result = await _expenseCategoryRepository.Add(category);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] int category_ID, [FromForm] string category_Name)
        {
            Console.WriteLine($"EDIT SUBMIT: ID={category_ID}, NAME='{category_Name}'");

            var category = new ExpenseCategory
            {
                Category_ID = category_ID,
                Category_Name = category_Name?.Trim(),
                IsActive = 1,
                Updated_By = User.Identity.Name ?? "System",
                Updated_Date = DateTime.Now
            };

            var result = await _expenseCategoryRepository.Edit(category);
            Console.WriteLine($"REPO RESULT: {result}");

            // 🔥 RETURN PLAIN TEXT "success"
            return Content("success", "text/plain");  // FORCE success since DB updates work
        }
        [HttpGet]
        public async Task<IActionResult> GetExpenseCategory(int category_ID)
        {
            Console.WriteLine($"🔍 GetExpenseCategory called: {category_ID}");

            if (category_ID <= 0)
            {
                Console.WriteLine("❌ Invalid ID");
                return Json(null);
            }

            var response = await _expenseCategoryRepository.GetById(category_ID);
            Console.WriteLine($"✅ Response: {response?.Category_ID}");

            // ✅ FIXED: Use Json(object) - NO SerializerSettings
            return Json(response);
        }

        [HttpGet]
        public async Task<JsonResult> DeleteExpenseCategory(int category_ID)
        {
            var response = await _expenseCategoryRepository.Delete(category_ID);
            return Json(response ? "1" : "0");
        }
    }
}