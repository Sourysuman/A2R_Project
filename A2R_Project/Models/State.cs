using System.ComponentModel.DataAnnotations;

namespace A2R_Project.Models
{
    public class State
    {
        [Key]
        public int StateID { get; set; }
        public int? CountryID { get; set; }
        public string? CountryName { get; set; }
        public string? StateCode { get; set; }
        public string? StateName { get; set; }
        public int? IsActive { get; set; }
        public int? IsDeleted { get; set; }
    }
}
