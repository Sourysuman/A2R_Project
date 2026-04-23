using A2R_Project.Context;
using A2R_Project.Interface;
using A2R_Project.Models;
using Dapper;
using System.Data;

namespace A2R_Project.Repositories
{
    public class AdmissionRepository : IAdmissionRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdmissionRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        private string GetCurrentUser()
        {
            var username = _httpContextAccessor.HttpContext?.Session.GetString("Username");
            return !string.IsNullOrEmpty(username) ? username : "System";
        }
        // ✅ YOUR EXISTING METHOD 1 - GetAllAdmission
        public async Task<List<Admission>> GetAllAdmission()
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var admissions = await connection.QueryAsync<Admission>(
                    "sp_GetAllAdmissions",
                    commandType: CommandType.StoredProcedure);
                return admissions.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllAdmission Error: {ex.Message}");
                throw;
            }
        }

        // ✅ YOUR EXISTING METHOD 2 - Add (USED BY OTHER PAGES)
        public async Task<string> Add(Admission admission)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@StudentInquiryID", admission.StudentInquiryID);
                parameters.Add("@Admission_Date", admission.Admission_Date);
                parameters.Add("@CourseID", admission.CourseID);
                parameters.Add("@Actual_Amount", admission.Actual_Amount);
                parameters.Add("@Discount_Amount", admission.Discount_Amount ?? 0);
                parameters.Add("@Fine_Amount", admission.Fine_Amount ?? 0);
                parameters.Add("@Net_Amount", admission.Net_Amount);
                parameters.Add("@Pending_Amount", admission.Pending_Amount);
                parameters.Add("@Paid_Amount", admission.Paid_Amount);
                parameters.Add("@Payment_Method", admission.Payment_Method);
                parameters.Add("@Payment_Remark", admission.Payment_Remark);
                parameters.Add("@Created_By", GetAllAdmission());
                parameters.Add("@Updated_By", GetAllAdmission());
                parameters.Add("@SequenceNo", admission.SequenceNo);

                parameters.Add("@ResultMessage", dbType: DbType.String,
                    direction: ParameterDirection.Output, size: 255);

                await connection.ExecuteAsync("sp_AddAdmission", parameters,
                    commandType: CommandType.StoredProcedure);

                string resultMessage = parameters.Get<string>("@ResultMessage")?.Trim() ?? "Error";
                Console.WriteLine($"Add Admission Result: {resultMessage}");
                return resultMessage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add Admission Error: {ex.Message}");
                throw ex;
            }
        }

        // ✅ YOUR EXISTING METHOD 3 - GetAdmissionList
        public async Task<List<Admission>> GetAdmissionList(string name, string fromDate, string toDate, int courseId, string phoneNo, string sequenceNo)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();

                var parameters = new
                {
                    Name = name,
                    from_date = fromDate,
                    to_date = toDate,
                    course_id = courseId,
                    phone_no = phoneNo,
                    sequence_no = sequenceNo
                };

                var results = await connection.QueryAsync<dynamic>(
                    "sp_getAdmissionList", parameters, commandType: CommandType.StoredProcedure);

                var admissions = new List<Admission>();
                foreach (var result in results)
                {
                    var admission = new Admission();

                    // YOUR exact date parsing logic
                    if (result.Admission_Date != null)
                    {
                        DateTime admissionDate;
                        if (DateTime.TryParseExact(result.Admission_Date.ToString(), "dd/MM/yyyy",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out admissionDate))
                        {
                            admission.Admission_Date = admissionDate;
                        }
                    }

                    // Map ALL your existing fields
                    admission.AdmissionID = result.AdmissionID;
                    admission.StudentInquiryID = result.StudentInquiryID;
                    admission.StudentName = result.NameOfStudent ?? result.StudentName;
                    admission.CourseNames = result.CourseName ?? result.CourseNames;
                    admission.Actual_Amount = result.Course_fee ?? result.Actual_Amount;
                    admission.Discount_Amount = result.Discount_Amount;
                    admission.Net_Amount = result.Net_Amount;
                    admission.Paid_Amount = result.Paid_Amount;
                    admission.Pending_Amount = result.Pending_Amount;
                    admission.Payment_Method = result.Payment_Type ?? result.Payment_Method;
                    admission.Payment_Remark = result.Payment_Remark;
                    admission.SequenceNo = result.SequenceNo;
                    admission.Education = result.Education;
                    admission.ContactNo = result.ContactNo;
                    admission.Email = result.Email ?? result.StudentEmail;
                    admission.Address2 = result.Address2;
                    admission.City = result.City;
                    admission.StateName = result.StateName;
                    admission.CountryName = result.CountryName;
                    admission.Fine_Amount = result.Fine_Amount;
                    admission.Created_By = result.Created_By;
                    admission.Created_Date = result.Created_Date;
                    admission.Updated_By = result.Updated_By;
                    admission.Updated_Date = result.Updated_Date;
                    admission.IsActive = result.IsActive;
                    admission.IsDeleted = result.IsDeleted;

                    admissions.Add(admission);
                }
                return admissions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAdmissionList Error: {ex.Message}");
                throw;
            }
        }

        // ✅ NEW METHOD - StudentsList Integration (Uses YOUR Add method)
        public async Task<string> AddAdmissionFromStudentsList(long studentInquiryID, Admission admissionData)
        {
            try
            {
                var admission = new Admission
                {
                    StudentInquiryID = studentInquiryID,
                    Admission_Date = admissionData.Admission_Date,
                    CourseID = admissionData.CourseID,
                    Actual_Amount = admissionData.Actual_Amount,
                    Discount_Amount = admissionData.Discount_Amount,
                    Fine_Amount = admissionData.Fine_Amount,
                    Net_Amount = admissionData.Net_Amount,
                    Pending_Amount = admissionData.Pending_Amount,
                    Paid_Amount = admissionData.Paid_Amount,
                    Payment_Method = admissionData.Payment_Method,
                    Payment_Remark = admissionData.Payment_Remark,
                    SequenceNo = admissionData.SequenceNo,
                    Created_By = admissionData.Created_By,
                    Updated_By = admissionData.Updated_By
                };

                // ✅ CALLS YOUR EXISTING Add METHOD
                return await Add(admission);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddAdmissionFromStudentsList Error: {ex.Message}");
                return "Error: " + ex.Message;
            }
        }
    }
}