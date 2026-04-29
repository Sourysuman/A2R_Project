using A2R_Project.Interfaces;
using A2R_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace A2R_Project.Controllers
{
    public class StateController : BaseController
    {
        private readonly IStateRepository _stateRepository;

        public StateController(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }
        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            try
            {
            
                var allStates = await _stateRepository.GetAllStates();
                var totalCount = allStates?.Count ?? 0;

                await SetPagePermissions("State");
                var pagedStates = allStates
                    ?.Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList() ?? new List<State>();

                // PASS STATES TO VIEWDATA
                ViewData["states"] = pagedStates;

                // Get countries for dropdown
                var countries = await _stateRepository.GetAllCountries();
                ViewBag.Countries = new SelectList(countries, "CountryID", "CountryName");

          
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;
                ViewBag.ShowAllButton = totalCount > pageSize; 
                ViewBag.HasPagination = totalCount > pageSize; 
                return View(new State());
            }
            catch (Exception ex)
            {
                // Set default values even on error
                ViewBag.Error = ex.Message;
                ViewBag.ShowAllButton = false;
                ViewBag.TotalCount = 0;
                ViewBag.CurrentPage = 1;
                ViewBag.PageSize = 10;
                ViewBag.TotalPages = 0;
                ViewBag.HasPagination = false;
                ViewData["states"] = new List<State>();
                return View(new State());
            }
        }
        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            try
            {
                var states = await _stateRepository.GetAllStates();
                return Json(states);
            }
            catch
            {
                return Json(new List<State>());
            }
        }

        [HttpPost]
        public async Task<JsonResult> Save(State stateData)
        {
            var response = await _stateRepository.Add(stateData);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> Edit(State stateData)
        {
            var response = await _stateRepository.Edit(stateData);
            return Json(response ? "success" : "");
        }

        [HttpGet]
        public async Task<JsonResult> GetState(string stateID)
        {
            try
            {
                var id = JsonConvert.DeserializeObject<int>(stateID);
                var response = await _stateRepository.GetById(id);
                return Json(response);
            }
            catch
            {
                return Json(null);
            }
        }

        [HttpGet]
        public async Task<JsonResult> DeleteState(string stateId)
        {
            try
            {
                var id = JsonConvert.DeserializeObject<int>(stateId);
                var response = await _stateRepository.Delete(id);
                return Json(response ? "1" : "0");
            }
            catch
            {
                return Json("0");
            }
        }
    }
}