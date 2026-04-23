using A2R_Project.Interface;
using A2R_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace A2R_Project.Controllers
{
    public class FeesCollectionController : BaseController
    {
        private readonly IAdmissionRepository _admissionRepository;

        public FeesCollectionController(IAdmissionRepository admissionRepository)
        {
            _admissionRepository = admissionRepository;
        }

        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            var allAdmissions = await _admissionRepository.GetAllAdmission();
            var totalCount = allAdmissions.Count;
            var pagedAdmissions = allAdmissions.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new List<Admission>();
            model.AddRange(pagedAdmissions);

            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(model);
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