using A2R_Project.Models;

namespace A2R_Project.Interface
{
    public interface IAdmissionRepository
    {
        Task<List<Admission>> GetAllAdmission();
        Task<string> Add(Admission admission);  // ✅ YOUR EXISTING
        Task<List<Admission>> GetAdmissionList(string name, string fromDate, string toDate, int courseId, string phoneNo, string sequenceNo); // ✅ YOUR EXISTING
        Task<string> AddAdmissionFromStudentsList(long studentInquiryID, Admission admissionData); // ✅ NEW FOR StudentsList
    }
}