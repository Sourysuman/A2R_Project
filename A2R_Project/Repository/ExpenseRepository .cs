using A2R_Project.Context;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace A2R_Project.Repository
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _uploadPath;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ExpenseRepository(AppDbContext dbContext, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
            _uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "ExpenseProof");
            _httpContextAccessor = httpContextAccessor;
        }
        // 🔥 ADD THIS METHOD RIGHT HERE
        private string GetCurrentUser()
        {
            var username = _httpContextAccessor.HttpContext?.Session.GetString("Username");
            return !string.IsNullOrEmpty(username) ? username : "Admin";
        }
        public async Task<List<Expense>> GetAllExpenses()
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var expenses = await connection.QueryAsync<Expense>("sp_GetAllExpenses", commandType: CommandType.StoredProcedure);
                return expenses.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllExpenses Error: {ex.Message}");
                return new List<Expense>();
            }
        }

        public async Task<Expense?> GetExpenseById(int expense_ID)
        {
            try
            {
                using var conn = _dbContext.CreateConnection();
                return await conn.QuerySingleOrDefaultAsync<Expense>(
                    "sp_ViewExpense",
                    new { Expense_ID = expense_ID },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetExpenseById Error: {ex.Message}");
                return null;
            }
        }

        public async Task<string> AddExpense(Expense expense, IFormFile? file)
        {
            try
            {
                // File upload
                if (file != null && file.Length > 0)
                {
                    if (!Directory.Exists(_uploadPath)) Directory.CreateDirectory(_uploadPath);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var fullPath = Path.Combine(_uploadPath, fileName);
                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(stream);
                    expense.Expense_Proof = $"/images/ExpenseProof/{fileName}";
                }

                using var connection = _dbContext.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@Category_ID", expense.Category_ID ?? (object)DBNull.Value);
                parameters.Add("@Expense_Title", expense.Expense_Title ?? "");
                parameters.Add("@Invoice_Id", expense.Invoice_Id ?? "");
                parameters.Add("@Expense_Amount", expense.Expense_Amount ?? 0);
                parameters.Add("@Expense_Date", expense.Expense_Date ?? (object)DBNull.Value);
                parameters.Add("@Expense_Proof", expense.Expense_Proof ?? "");
                parameters.Add("@Expense_Remark", expense.Expense_Remark ?? "");
                parameters.Add("@Created_By", GetCurrentUser());
                parameters.Add("@ResultMessage", "", DbType.String, ParameterDirection.Output, size: -1);  // ✅ nvarchar(max)

                await connection.ExecuteAsync("sp_AddExpense", parameters, commandType: CommandType.StoredProcedure);
                var result = parameters.Get<string>("@ResultMessage") ?? "success";

                Console.WriteLine($"✅ Add result: {result}");
                return result;  // "success" or "duplicate"
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Add error: {ex.Message}");
                return "error: " + ex.Message;
            }
        }

        public async Task<string> EditExpense(Expense expense, IFormFile? file, string existingImagePath = "")
        {
            try
            {
                // Delete old image if new file
                if (file != null && file.Length > 0 && !string.IsNullOrEmpty(existingImagePath))
                {
                    var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, existingImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                // New image upload
                if (file != null && file.Length > 0)
                {
                    if (!Directory.Exists(_uploadPath)) Directory.CreateDirectory(_uploadPath);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var fullPath = Path.Combine(_uploadPath, fileName);
                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(stream);
                    expense.Expense_Proof = $"/images/ExpenseProof/{fileName}";
                }
                else
                {
                    expense.Expense_Proof = existingImagePath ?? "";
                }

                using var conn = _dbContext.CreateConnection();
                var p = new DynamicParameters();
                p.Add("@Expense_ID", expense.Expense_ID);
                p.Add("@Category_ID", expense.Category_ID ?? (object)DBNull.Value);
                p.Add("@Expense_Title", expense.Expense_Title ?? "");
                p.Add("@Invoice_Id", expense.Invoice_Id ?? "");
                p.Add("@Expense_Amount", expense.Expense_Amount ?? 0);
                p.Add("@Expense_Date", expense.Expense_Date ?? (object)DBNull.Value);
                p.Add("@Expense_Proof", expense.Expense_Proof ?? "");
                p.Add("@Expense_Remark", expense.Expense_Remark ?? "");
                p.Add("@Updated_By", GetCurrentUser());
                // ✅ NO ResultMessage - Your SP doesn't have it

                await conn.ExecuteAsync("sp_EditExpense", p, commandType: CommandType.StoredProcedure);
                return "success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Edit error: {ex.Message}");
                return "error: " + ex.Message;
            }
        }
        public async Task<string> DeleteExpense(int expense_ID)
        {
            try
            {
                using var conn = _dbContext.CreateConnection();

                // ✅ Get image path first
                var expense = await conn.QuerySingleOrDefaultAsync<dynamic>(
                    "SELECT Expense_Proof FROM Expense WHERE Expense_ID = @Expense_ID AND IsDeleted = 0",
                    new { Expense_ID = expense_ID });

                // ✅ Delete physical image
                if (expense?.Expense_Proof != null)
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                        expense.Expense_Proof.ToString().TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                        Console.WriteLine("🗑️ Image deleted");
                    }
                }

                // ✅ Simple soft delete
                await conn.ExecuteAsync("sp_DeleteExpense",
                    new { Expense_ID = expense_ID },
                    commandType: CommandType.StoredProcedure);

                return "success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DeleteExpense Error: {ex.Message}");
                return "error: " + ex.Message;
            }
        }
    }
}