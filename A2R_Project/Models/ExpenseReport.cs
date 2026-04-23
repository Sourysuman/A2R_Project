using System.ComponentModel.DataAnnotations;

namespace A2R_Project.Models
{
    public class ExpenseReport
    {
        [Key]
        public int Category_ID { get; set; }
        public string? Category_Name { get; set; }
        public string? Expense_Type { get; set; }
        public DateTime? Expense_Date { get; set; }
        public decimal? Expense_Amount { get; set; }
        public string? expense_statues { get; set; }
    }
    public class CategoryViewModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}