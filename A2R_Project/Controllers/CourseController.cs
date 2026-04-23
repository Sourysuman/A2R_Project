using A2R_Project.Controllers;
using A2R_Project.Models;

using A2RSystemWebApp.Interfaces;
using A2RSystemWebApp.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace A2RSystemWebApp.Controllers
{
    public class CourseController : BaseController
    {
        private readonly ICourseRepository _courseRepository;

        public CourseController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            try
            {
                var allCourses = await _courseRepository.GetAllCourses();
                allCourses = allCourses.OrderByDescending(c => c.CourseID).ToList();

                var totalCount = allCourses.Count;
                var pagedCourses = allCourses.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.TotalCount = totalCount;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.ShowAllButton = totalCount > pageSize;

                return View(pagedCourses);
            }
            catch
            {
                return View(new List<Course>());
            }
        }

        // In CourseController.cs - REPLACE existing methods with these:

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] Course course)
        {
            if (course == null || string.IsNullOrWhiteSpace(course.CourseName))
                return Json(new { success = false, message = "Invalid course data" });

            try
            {
                var result = await _courseRepository.Add(course);
                return Json(new { success = true, message = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] Course course)
        {
            if (course?.CourseID <= 0 || string.IsNullOrWhiteSpace(course.CourseName))
                return Json(new { success = false, message = "Invalid course data" });

            try
            {
                var result = await _courseRepository.Edit(course);
                return Json(new { success = result, message = result ? "Updated successfully" : "Update failed" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int courseID)
        {
            if (courseID <= 0)
                return Json(new { success = false, message = "Invalid course ID" });

            try
            {
                var result = await _courseRepository.Delete(courseID);
                return Json(new { success = result, message = result ? "Deleted successfully" : "Delete failed" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            try
            {
                var courses = await _courseRepository.GetAllCourses();
                if (courses != null && courses.Any())
                {
                    courses = courses.OrderByDescending(c => c.CourseID).ToList();
                }
                return Json(courses ?? new List<Course>());
            }
            catch
            {
                return Json(new List<Course>());
            }
        }
    }
}