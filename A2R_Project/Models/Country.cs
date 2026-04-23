using System.ComponentModel.DataAnnotations;

namespace A2R_Project.Models
{
    public class Country
    {
        [Key]
        public int? CountryID { get; set; }
        public string? CountryName { get; set; }
        public int? IsDeleted { get; set; }
        public int? IsActive { get; set; }
    }
}
