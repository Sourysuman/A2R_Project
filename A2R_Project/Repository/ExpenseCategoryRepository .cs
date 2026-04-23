using A2R_Project.Interface;  // ✅ Interface namespace
using A2R_Project.Models;       // ✅ Model namespace
using A2R_Project.Context;

using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace A2R_Project.Repositories
{
    public class ExpenseCategoryRepository : IExpenseCategoryRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExpenseCategoryRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentUser()
        {
            var username = _httpContextAccessor.HttpContext?.Session.GetString("Username");
            return !string.IsNullOrEmpty(username) ? username : "Admin";
        }
        public async Task<List<ExpenseCategory>> GetAllExpenseCategories()
        {
            using var connection = _dbContext.CreateConnection();
            var expenseCategories = await connection.QueryAsync<ExpenseCategory>("sp_GetAllExpenseCategories", commandType: CommandType.StoredProcedure);
            return expenseCategories.ToList();
        }
        public async Task<string> Add(ExpenseCategory expenseCategory)
        {
            using var connection = _dbContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Category_Name", expenseCategory.Category_Name, DbType.String, size: 100);
            parameters.Add("@IsActive", expenseCategory.IsActive ?? 1);
            parameters.Add("@Created_By", GetCurrentUser(), DbType.String, size: 50);
            parameters.Add("@ResultMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            await connection.ExecuteAsync("sp_AddExpenseCategory", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<string>("@ResultMessage") ?? "Failed";
        }
        public async Task<bool> Edit(ExpenseCategory expenseCategory)
        {
            using var connection = _dbContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Category_ID", expenseCategory.Category_ID);
            parameters.Add("@Category_Name", expenseCategory.Category_Name, DbType.String, size: 100);
            parameters.Add("@IsActive", expenseCategory.IsActive ?? 1);
            parameters.Add("@Updated_By", GetCurrentUser() ,DbType.String, size: 50);
            parameters.Add("@Updated_Date", expenseCategory.Updated_Date ?? DateTime.Now);

            var rows = await connection.ExecuteAsync("sp_EditExpenseCategory", parameters, commandType: CommandType.StoredProcedure);
            Console.WriteLine($"Rows affected: {rows}");  // ✅ Debug log
            return rows > 0;
        }

        public async Task<ExpenseCategory> GetById(int category_ID)
        {
            using var connection = _dbContext.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<ExpenseCategory>("sp_ViewExpenseCategory", new { Category_ID = category_ID }, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> Delete(int category_ID)
        {
            using var connection = _dbContext.CreateConnection();
            int rowAffected = await connection.ExecuteAsync("sp_DeleteExpenseCategory", new { Category_ID = category_ID }, commandType: CommandType.StoredProcedure);
            return rowAffected > 0;
        }
    }
}