using A2R_Project.Context;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using Dapper;
using System.Data;

namespace A2R_Project.Repositories
{
    public class StudentInquiryRepository : IStudentInquiryRepository
    {
        private readonly AppDbContext _dbContext;

        public StudentInquiryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<StudentInquiry>> GetAllStudentInquiries()
        {
            using var connection = _dbContext.CreateConnection();
            var result = await connection.QueryAsync<StudentInquiry>("sp_GetAllStudentInquiries", commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<string> Add(StudentInquiry studentInquiry)
        {
            using var connection = _dbContext.CreateConnection();
            var parameters = new DynamicParameters();

            // ✅ NULL-SAFE PARAMETERS
            parameters.Add("NameOfStudent", studentInquiry.NameOfStudent ?? "");
            parameters.Add("FathersName", studentInquiry.FathersName ?? "");
            parameters.Add("CompanyName", studentInquiry.CompanyName ?? "");
            parameters.Add("Education", studentInquiry.Education ?? "");
            parameters.Add("ContactNo", studentInquiry.ContactNo ?? "");
            parameters.Add("Designation", studentInquiry.Designation ?? "");
            parameters.Add("Email", studentInquiry.Email ?? "");
            parameters.Add("StateID", studentInquiry.StateID ?? 0);
            parameters.Add("CountryID", studentInquiry.CountryID ?? 0);
            parameters.Add("Address1", studentInquiry.Address1 ?? "");
            parameters.Add("Address2", studentInquiry.Address2 ?? "");
            parameters.Add("City", studentInquiry.City ?? "");
            parameters.Add("Postcode", studentInquiry.Postcode ?? "");

            parameters.Add("LeadType", studentInquiry.LeadType ?? "");
            parameters.Add("EnrolledDate", studentInquiry.EnrolledDate ?? (DateTime?)null);
            parameters.Add("CompletionDate", studentInquiry.CompletionDate ?? (DateTime?)null);

            parameters.Add("CourseID", studentInquiry.CourseID ?? 0);
            parameters.Add("IsActive", studentInquiry.IsActive ?? 1);
            parameters.Add("IsDeleted", studentInquiry.IsDeleted ?? 0);
            parameters.Add("ResultMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 4000);

            await connection.ExecuteAsync("sp_AddStudentInquiry", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<string>("ResultMessage") ?? "Error";
        }

        public async Task<bool> Edit(StudentInquiry studentInquiry)
        {
            using var connection = _dbContext.CreateConnection();
            var parameters = new
            {
                StudentInquiryID = studentInquiry.StudentInquiryID,
                studentInquiry.NameOfStudent,
                studentInquiry.FathersName,
                studentInquiry.CompanyName,
                studentInquiry.Education,
                studentInquiry.ContactNo,
                studentInquiry.Designation,
                studentInquiry.Email,
                studentInquiry.StateID,
                studentInquiry.CountryID,
                studentInquiry.Address1,
                studentInquiry.Address2,
                studentInquiry.City,
                studentInquiry.Postcode,
                studentInquiry.EnrolledDate,
                studentInquiry.CompletionDate,
                studentInquiry.CourseID,
                studentInquiry.IsActive,
                studentInquiry.IsDeleted
            };
            int rows = await connection.ExecuteAsync("sp_EditStudentInquiry", parameters, commandType: CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<StudentInquiry> GetById(int studentInquiryID)
        {
            using var connection = _dbContext.CreateConnection();
            var parameters = new { studentInquiryID };
            return await connection.QuerySingleOrDefaultAsync<StudentInquiry>("sp_ViewStudentInquiry", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> Delete(int studentInquiryID)
        {
            using var connection = _dbContext.CreateConnection();
            int rows = await connection.ExecuteAsync("sp_DeleteStudentInquiry", new { StudentInquiryID = studentInquiryID }, commandType: CommandType.StoredProcedure);
            return rows > 0;
        }
    }
}