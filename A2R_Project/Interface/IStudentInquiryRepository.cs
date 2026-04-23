using A2R_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace A2R_Project.Interfaces
{
    public interface IStudentInquiryRepository
    {
        Task<List<StudentInquiry>> GetAllStudentInquiries();
        Task<string> Add(StudentInquiry studentInquiry);
        Task<bool> Edit(StudentInquiry studentInquiry);
        Task<StudentInquiry> GetById(int studentInquiryID);
        Task<bool> Delete(int studentInquiryID);
    }
}