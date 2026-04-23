using System.ComponentModel.DataAnnotations;

namespace A2R_Project.Models
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }
        public int? CompanyID { get; set; }
        public string? BranchName { get; set; }
        public string? BranchAddress1 { get; set; }
        public string? BranchAddress2 { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? Pincode { get; set; }
        public string? PANNo { get; set; }
        public string? GSTNo { get; set; }
        public string? TANNo { get; set; }
        public string? VATNo { get; set; }
        public string? PTTaxNo { get; set; }
        public string? PFNo { get; set; }
        public string? Email { get; set; }
        public string? ContactNo { get; set; }
        public string? BranchCity { get; set; }
        public int? BranchStateId { get; set; }
        public int? BranchCountryId { get; set; }
        public string? BranchPincode { get; set; }
        public string? BranchPanNo { get; set; }
        public string? BranchGstNo { get; set; }
        public string? BranchTanNo { get; set; }
        public string? BranchVatNo { get; set; }
        public string? BranchPttaxNo { get; set; }
        public string? BranchPfNo { get; set; }
        public string? BranchEmail { get; set; }
        public string? BranchContactNo { get; set; }
        public string? CompanyName { get; set; }
        public string? StateName { get; set; }
        public string? CountryName { get; set; }
        public int? IsDeleted { get; set; }
        public int? IsActive { get; set; }
    }

    public class BranchViewModel
    {
        public List<Branch> branches { get; set; } = new List<Branch>();
        public List<Company> companies { get; set; } = new List<Company>();
        public List<State> states { get; set; } = new List<State>();
        public List<Country> country { get; set; } = new List<Country>();
    }
}