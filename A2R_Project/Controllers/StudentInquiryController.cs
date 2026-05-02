using A2R_Project.Interface;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using A2RSystemWebApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace A2R_Project.Controllers
{
    public class StudentInquiryController : BaseController
    {
        private readonly IStudentInquiryRepository _studentInquiryService;
        private readonly IStateRepository _state;
        private readonly ICountryRepository _country;
        private readonly ICourseRepository _course;

        public StudentInquiryController(
            IStudentInquiryRepository studentInquiryService,
            IStateRepository state,
            ICountryRepository country,
            ICourseRepository course)
        {
            _studentInquiryService = studentInquiryService;
            _state = state;
            _country = country;
            _course = course;
        }

        public async Task<IActionResult> List()
        {
            await SetPagePermissions("StudentInquiry");
            var models = new StudentInquiries();
            models.StudentInquiry = await _studentInquiryService.GetAllStudentInquiries();
            models.state = await _state.GetAllStates();
            models.country = await _country.GetAllCountries();
            models.course = await _course.GetAllCourses();
            return View(models);
        }

        [HttpPost]
        public async Task<JsonResult> SaveSaveInquiry(StudentInquiry studentInquiryData)
        {
            try
            {
                await SetPagePermissions("StudentInquiry");
                if (studentInquiryData == null)
                    return Json("Error: No data");

                var response = await _studentInquiryService.Add(studentInquiryData);
                return Json(response ?? "success");
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message);
            }
        }
    }
}