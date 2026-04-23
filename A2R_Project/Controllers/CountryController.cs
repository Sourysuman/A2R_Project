using A2R_Project.Interface;
using A2R_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace A2R_Project.Controllers
{
    public class CountryController : BaseController
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ILogger<CountryController> _logger;

        public CountryController(ICountryRepository countryRepository, ILogger<CountryController> logger)
        {
            _countryRepository = countryRepository;
            _logger = logger;
        }

        // GET: /Country/List
        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            try
            {
                var models = new List<Country>();
                var allCountries = await _countryRepository.GetAllCountries();

                // ✅ FIXED: Filter out deleted + Order by ID DESC (Recent First)
                var activeCountries = allCountries
                    .Where(c => c.IsDeleted != 1)
                    .OrderByDescending(c => c.CountryID) // Recently added first
                    .ToList();

                var totalCount = activeCountries.Count;
                var pagedCountries = activeCountries
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                models.AddRange(pagedCountries);

                ViewBag.TotalCount = totalCount;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.ShowAllButton = totalCount > pageSize;

                return View(models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading countries list: {Message}", ex.Message);
                ViewBag.Error = "Error loading data. Please try again.";
                return View(new List<Country>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            try
            {
                var countries = await _countryRepository.GetAllCountries();
                // ✅ Return ALL countries (including deleted for admin view)
                var styledCountries = countries
                    .OrderByDescending(c => c.CountryID) // Recent first
                    .Select(c => new {
                        CountryID = c.CountryID,
                        CountryName = c.CountryName,
                        IsActive = c.IsActive,
                        IsDeleted = c.IsDeleted,
                        IsRecent = c.CountryID > (countries.Max(x => x.CountryID) - 5) // Last 5 = recent
                    }).ToList();

                return Json(styledCountries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ShowAll");
                return Json(new { error = "Failed to load data" });
            }
        }
        // POST: /Country/Save
        [HttpPost]
        public async Task<JsonResult> Save(Country countryData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(countryData?.CountryName))
                {
                    return Json("Please enter country name");
                }

                countryData.IsDeleted ??= 0;
                countryData.IsActive ??= 1;

                var response = await _countryRepository.SaveCountry(countryData);
                response = response.Trim().ToLower();

                if (response == "success")
                {
                    return Json("success");
                }
                else if (response == "duplicate")
                {
                    return Json("duplicate");
                }
                else
                {
                    return Json("Operation failed: " + response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving country");
                return Json("Error: " + ex.Message);
            }
        }

        // POST: /Country/Edit
        [HttpPost]
        public async Task<JsonResult> Edit(Country countryData)
        {
            try
            {
                if (countryData?.CountryID == null || string.IsNullOrWhiteSpace(countryData.CountryName))
                {
                    return Json("Invalid data");
                }

                countryData.IsDeleted ??= 0;

                var response = await _countryRepository.UpdateCountries(countryData);

                if (response == "success")
                {
                    return Json("success");
                }
                else
                {
                    return Json("Update failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating country");
                return Json("Error: " + ex.Message);
            }
        }

        // GET: /Country/GetCountries?CountryID=...
        [HttpGet]
        public async Task<JsonResult> GetCountries(string CountryID)
        {
            try
            {
                if (string.IsNullOrEmpty(CountryID))
                {
                    return Json(null);
                }

                var result = JsonConvert.DeserializeObject<string>(CountryID);
                var response = await _countryRepository.GetCountries(result);
                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting country by ID");
                return Json(null);
            }
        }

        // GET: /Country/DeleteCountry?countryId=...
        [HttpGet]
        public async Task<JsonResult> DeleteCountry(string countryId)
        {
            try
            {
                if (string.IsNullOrEmpty(countryId))
                {
                    return Json("Invalid country ID");
                }

                var response = await _countryRepository.DeleteCountry(countryId);

                if (response == "success")
                {
                    return Json("success");
                }
                else
                {
                    return Json("Delete failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting country");
                return Json("Error: " + ex.Message);
            }
        }
    }
}