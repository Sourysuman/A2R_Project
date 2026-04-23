using System.ComponentModel.DataAnnotations;

namespace A2R_Project.Models
{
    public class Expense
    {
        [Key]
        public int Expense_ID { get; set; }
        public int? Category_ID { get; set; }
        public string? Category_Name { get; set; }
        public string? Expense_Title { get; set; }
        public string? Invoice_Id { get; set; }
        public decimal? Expense_Amount { get; set; }
        public DateTime? Expense_Date { get; set; }
        public string? Expense_Proof { get; set; }
        public string? ExistingExpense_Proof { get; set; }
        public string? Expense_Remark { get; set; }
        public int? IsActive { get; set; }
        public int? IsDeleted { get; set; }
        public string? Created_By { get; set; }
        public DateTime? Created_Date { get; set; }
        public string? Updated_By { get; set; }
        public DateTime? Updated_Date { get; set; }
    }

    public class ExpenseViewModel
    {
        public List<Expense> expenses { get; set; } = new List<Expense>();
        public List<ExpenseCategory> expenseCategory { get; set; } = new List<ExpenseCategory>();
    }

  
}