using System.ComponentModel.DataAnnotations;

namespace A2R_Project.Models
{
    public class Company
    {
        [Key]
        public int CompanyID { get; set; }
        public string? Comp_Name { get; set; }
        public string? Comp_Address1 { get; set; }
        public string? Comp_Address2 { get; set; }
        public string? Comp_Area { get; set; }
        public string? Comp_City { get; set; }
        public int? Comp_State_ID { get; set; }
        public int? Comp_Country_ID { get; set; }
        public string? Comp_Pincode { get; set; }
        public string? Comp_PAN_No { get; set; }
        public string? Comp_TAN_No { get; set; }
        public string? Comp_TIN_No { get; set; }
        public string? Comp_VAT_No { get; set; }
        public string? Comp_GST_No { get; set; }
        public string? Comp_Logo_Name { get; set; }
        public string? Comp_PTTax_No { get; set; }
        public string? Comp_PF_No { get; set; }
        public string? Comp_Email { get; set; }
        public string? Comp_Website { get; set; }
        public string? Comp_Contact_No { get; set; }
        public string? SMTP_Server_Name { get; set; }
        public string? SMTP_Port_No { get; set; }
        public string? SMTP_User_Name { get; set; }
        public string? SMTP_User_Password { get; set; }
        public string? Comp_CIN_No { get; set; }
        public int? IsActive { get; set; }
        public int? IsDelete { get; set; }
        public string? StateName { get; set; }
        public string? CountryName { get; set; }
        public string? CompanyName { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Area { get; set; }
        public string? City { get; set; }
        public string? Pincode { get; set; }
        public string? PANNo { get; set; }
        public string? TANNo { get; set; }
        public string? TINNo { get; set; }
        public string? VATNo { get; set; }
        public string? GSTNo { get; set; }
        public string? LogoName { get; set; }
        public string? PTTaxNo { get; set; }
        public string? PFNo { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? ContactNo { get; set; }
        public int? StartYear { get; set; }
        public string? SMTPServerName { get; set; }
        public string? SMTPPortNo { get; set; }
        public string? SMTPUserName { get; set; }
        public string? SMTPUserPassword { get; set; }
        public string? CINNo { get; set; }
        public string? PicturePath { get; set; }
        public string? ExistingPicturePath { get; set; }
    }
}