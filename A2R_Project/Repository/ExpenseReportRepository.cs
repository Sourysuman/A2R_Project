using A2R_Project.Context;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using Dapper;
using System.Data;

namespace A2R_Project.Repository
{
    public class ExpenseReportRepository : IExpenseReportRepository
    {
        private readonly AppDbContext _dbContext;

        public ExpenseReportRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ExpenseReport>> GetAllExpenseReport()
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var expenseReports = await connection.QueryAsync<ExpenseReport>("sp_GetAllExpensesReport", commandType: CommandType.StoredProcedure);
                return expenseReports.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ExpenseReport>> GetFilteredExpenseReport(string expenseType, string category, DateTime? fromDate, DateTime? toDate, string invoice_Id)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@Expense_Type", expenseType ?? (object)DBNull.Value);
                parameters.Add("@Category", category ?? (object)DBNull.Value);
                parameters.Add("@FromDate", fromDate ?? (object)DBNull.Value);
                parameters.Add("@ToDate", toDate ?? (object)DBNull.Value);
                parameters.Add("@Invoice_ID", invoice_Id ?? (object)DBNull.Value);

                var expenseReports = await connection.QueryAsync<ExpenseReport>("sp_getExpenseReportFilter", parameters, commandType: CommandType.StoredProcedure);
                return expenseReports.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}