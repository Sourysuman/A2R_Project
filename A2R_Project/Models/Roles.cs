using System.ComponentModel.DataAnnotations;

namespace A2R_Project.Models
{
    public class Role
    {
        [Key]
        public int Role_ID { get; set; }
        public string? Role_Name { get; set; }
        public int? IsActive { get; set; }
        public int? IsDeleted { get; set; }
    }
}