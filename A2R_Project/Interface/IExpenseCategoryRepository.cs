using A2R_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A2R_Project.Interface
{
    public interface IExpenseCategoryRepository
    {
        Task<List<ExpenseCategory>> GetAllExpenseCategories();
        Task<string> Add(ExpenseCategory expenseCategory);  // Returns string
        Task<bool> Edit(ExpenseCategory expenseCategory);   // Returns bool
        Task<ExpenseCategory> GetById(int category_ID);
        Task<bool> Delete(int category_ID);
    }
}