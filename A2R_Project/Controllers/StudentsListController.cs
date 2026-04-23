using A2R_Project.Interface;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using A2R_Project.Repositories;
using A2RSystemWebApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace A2R_Project.Controllers
{
    public class StudentsListController : BaseController
    {
        private readonly IFollowUpRepository _followUpRepository;

        private readonly IStudentInquiryRepository _studentInquiryRepository;
        private readonly IStateRepository _stateRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IAdmissionRepository _admissionRepository;



        public StudentsListController(
            IStudentInquiryRepository studentInquiryRepository,
            IStateRepository stateRepository,
            ICountryRepository countryRepository,
            ICourseRepository courseRepository,
            IFollowUpRepository followUpRepository,
            IAdmissionRepository admissionRepository)
        {
            _studentInquiryRepository = studentInquiryRepository;
            _stateRepository = stateRepository;
            _countryRepository = countryRepository;
            _courseRepository = courseRepository;
             _followUpRepository = followUpRepository;
            _admissionRepository = admissionRepository;
        }

        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            var models = new StudentInquiries();
            var allStudentInquiries = await _studentInquiryRepository.GetAllStudentInquiries();

            // 🔥 ONLY THIS LINE CHANGED - NEWEST FIRST!
            var orderedStudentInquiries = allStudentInquiries.OrderByDescending(x => x.StudentInquiryID).ToList();

            var totalCount = orderedStudentInquiries.Count;
            var pagedStudentInquiries = orderedStudentInquiries.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            models.StudentInquiry = pagedStudentInquiries;
            models.state = await _stateRepository.GetAllStates();
            models.country = await _countryRepository.GetAllCountries();
            models.course = await _courseRepository.GetAllCourses();

            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(models);
        }

        [HttpPost]
        public async Task<JsonResult> Edit([FromBody] StudentInquiry studentInquiryData)
        {
            try
            {
                if (studentInquiryData == null || studentInquiryData.StudentInquiryID <= 0)
                    return Json("Error: Invalid data");

                bool isUpdated = await _studentInquiryRepository.Edit(studentInquiryData);
                if (isUpdated)
                {
                    return Json("success");
                }
                return Json("Update failed");
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetStudentInquiry(int studentInquiryID)
        {
            try
            {
                if (studentInquiryID <= 0)
                    return Json(new { error = "Invalid ID" });

                var studentInquiry = await _studentInquiryRepository.GetById(studentInquiryID);
                if (studentInquiry != null)
                {
                    return Json(studentInquiry);
                }
                return Json(new { error = "Student not found" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }



        [HttpPost]
        public async Task<JsonResult> SaveFollowUp([FromBody] FollowUpDetail followUpData)
        {
            try
            {
                if (followUpData.StudentInquiryID <= 0)
                    return Json("Invalid student ID");

                bool isUpdated = await _followUpRepository.UpdateFollowUp(followUpData);
                return Json(isUpdated ? "success" : "Update failed");
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message);
            }
        }


        [HttpPost]
        public async Task<JsonResult> SaveStudentAdmission([FromBody] Admission admissionData)
        {
            try
            {
                Console.WriteLine($"Admission StudentID: {admissionData.StudentInquiryID}");

                if (admissionData.StudentInquiryID <= 0)
                    return Json("Invalid student ID");

                // ✅ Uses YOUR existing Add method
                string result = await _admissionRepository.Add(admissionData);
                Console.WriteLine($"Admission Result: {result}");

                return Json(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Admission Error: {ex.Message}");
                return Json("Error: " + ex.Message);
            }
        }







    }
}


