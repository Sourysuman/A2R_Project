using A2R_Project.Models;

namespace A2R_Project.Interfaces
{
    public interface IExpenseReportRepository
    {
        Task<List<ExpenseReport>> GetAllExpenseReport();
        Task<List<ExpenseReport>> GetFilteredExpenseReport(string expenseType, string category, DateTime? fromDate, DateTime? toDate, string invoice_Id);
    }
}