using System.ComponentModel.DataAnnotations;

namespace A2R_Project.Models
{
    public class ExpenseCategory
    {
        [Key]
        public int Category_ID { get; set; }
        public string? Category_Name { get; set; }
        public int? IsActive { get; set; }
        public int? IsDeleted { get; set; }
        public string? Created_By { get; set; }
        public DateTime? Created_Date { get; set; }
        public string? Updated_By { get; set; }
        public DateTime? Updated_Date { get; set; }
    }
}