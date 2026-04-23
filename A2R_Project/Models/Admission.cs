using System;
using System.ComponentModel.DataAnnotations;

namespace A2R_Project.Models
{
    public class Admission
    {
        [Key]
        public long AdmissionID { get; set; }
        public long? StudentInquiryID { get; set; }
        public DateTime? Admission_Date { get; set; }
        public long? CourseID { get; set; }
        public long? Actual_Amount { get; set; }
        public long? Discount_Amount { get; set; }
        public long? Fine_Amount { get; set; }
        public long? Net_Amount { get; set; }
        public long? Pending_Amount { get; set; }
        public string? Payment_Method { get; set; }
        public string? Payment_Remark { get; set; }
        public string? Created_By { get; set; }
        public DateTime? Created_Date { get; set; }
        public string? Updated_By { get; set; }
        public DateTime? Updated_Date { get; set; }
        public int? IsActive { get; set; }
        public int? IsDeleted { get; set; }
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public string? StudentNo { get; set; }
        public string? CourseNames { get; set; }
        public long? Paid_Amount { get; set; }
        public string? SequenceNo { get; set; }
        public string? Education { get; set; }
        public string? ContactNo { get; set; }
        public string? Email { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? StateName { get; set; }
        public string? CountryName { get; set; }

    }
}
