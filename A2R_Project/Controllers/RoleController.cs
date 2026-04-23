using A2R_Project.Context;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using A2R_Project.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace A2R_Project.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            var roles = await _roleRepository.GetAllRoles();
            var totalCount = roles.Count;
            var pagedRoles = roles.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            return View(pagedRoles);
        }
        [HttpPost]
        public async Task<JsonResult> Save([FromBody] Role roleData)
        {
            if (roleData == null) return Json("Error: Invalid data");
            var response = await _roleRepository.Add(roleData);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> Edit([FromBody] Role roleData)
        {
            if (roleData == null || roleData.Role_ID <= 0) return Json("failed");
            var isUpdated = await _roleRepository.Edit(roleData);
            return Json(isUpdated ? "success" : "failed");
        }

        [HttpGet]
        public async Task<JsonResult> GetRole(string role_ID)
        {
            try
            {
                var roleId = JsonConvert.DeserializeObject<int>(role_ID);
                var response = await _roleRepository.GetById(roleId);
                return Json(response);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpGet]
        public async Task<JsonResult> DeleteRole(string role_ID)
        {
            try
            {
                var roleId = JsonConvert.DeserializeObject<int>(role_ID);
                var isDeleted = await _roleRepository.Delete(roleId);
                return Json(isDeleted ? "1" : "0");
            }
            catch
            {
                return Json("0");
            }
        }
    }
}