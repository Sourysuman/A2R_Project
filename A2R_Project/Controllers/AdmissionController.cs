using A2R_Project.Interface;
using A2R_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace A2R_Project.Controllers
{
    public class AdmissionController : BaseController
    {
        private readonly IAdmissionRepository _admissionRepository;

        public AdmissionController(IAdmissionRepository admissionRepository)
        {
            _admissionRepository = admissionRepository;
        }

        public async Task<IActionResult> List()
        {
            var admissions = await _admissionRepository.GetAllAdmission();
            ViewBag.TotalCount = admissions.Count;
            return View(admissions);
        }

        [HttpGet]
        public async Task<IActionResult> GetAdmissionList(
            [FromQuery] string name = null,
            [FromQuery] string fromDate = null,
            [FromQuery] string toDate = null,
            [FromQuery] int? courseId = null,
            [FromQuery] string phoneNo = null,
            [FromQuery] string sequenceNo = null)
        {
            try
            {
                var admissions = await _admissionRepository.GetAdmissionList(
                    name, fromDate, toDate, courseId ?? 0, phoneNo, sequenceNo);
                return Json(admissions);
            }
            catch (Exception ex)
            {
                return Json(new List<Admission>());
            }
        }
    }
}