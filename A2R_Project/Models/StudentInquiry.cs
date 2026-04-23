using A2R_Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2R_Project.Models
{
    public class StudentInquiries
    {
        public StudentInquiries()
        {
            this.StudentInquiry = new List<StudentInquiry>();
            this.state = new List<State>();
            this.country = new List<Country>();
            this.course = new List<Course>();
        }

        public List<StudentInquiry> StudentInquiry { get; set; }
        public List<State> state { get; set; }
        public List<Country> country { get; set; }
        public List<Course> course { get; set; }
    }

    public class StudentInquiry
    {
        public int StudentInquiryID { get; set; }
        public string? NameOfStudent { get; set; }
        public string? FathersName { get; set; }
        public string? CompanyName { get; set; }
        public string? Education { get; set; }
        public string? ContactNo { get; set; }
        public string? Designation { get; set; }
        public string? Email { get; set; }
        public int? StateID { get; set; }
        public string? State { get; set; }
        public int? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? Postcode { get; set; }
        public DateTime? EnrolledDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int? CourseID { get; set; }  // Make sure this is int? not string
        public string? Course { get; set; }
        public int? IsActive { get; set; }
        public int? IsDeleted { get; set; }
        public string? FollowUpDesc1 { get; set; }
        public string? FollowUpDesc2 { get; set; }
        public DateTime? FollowUpDate1 { get; set; }
        public DateTime? FollowUpDate2 { get; set; }
        public int? student_Inquiry_ind { get; set; }
    }
}