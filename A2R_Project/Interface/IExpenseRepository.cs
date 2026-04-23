using A2R_Project.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A2R_Project.Interfaces
{
    public interface IExpenseRepository
    {
        Task<List<Expense>> GetAllExpenses();
        Task<string> AddExpense(Expense expense, IFormFile? file);
        Task<string> EditExpense(Expense expense, IFormFile? file, string existingImagePath = ""); // ✅ FIXED
        Task<Expense?> GetExpenseById(int expense_ID);
        Task<string> DeleteExpense(int expense_ID); // ✅ FIXED
    }
}