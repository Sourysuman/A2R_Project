using A2R_Project.Context;
using A2R_Project.Interface;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using A2R_Project.Repositories;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace A2R_Project.Controllers
{
    public class CompanyController : BaseController
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IStateRepository _stateRepository;  // ✅ INJECTED
        private readonly ICountryRepository _countryRepository;  // ✅ INJECTED
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompanyController(
            ICompanyRepository companyRepository,
            IStateRepository stateRepository,      // ✅ PROPER INJECTION
            ICountryRepository countryRepository,  // ✅ PROPER INJECTION
            AppDbContext dbContext,
            IWebHostEnvironment webHostEnvironment)
        {
            _companyRepository = companyRepository;
            _stateRepository = stateRepository;
            _countryRepository = countryRepository;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            var model = new CompanyViewModel();  // Use ViewModel for dropdowns

            var allCompanies = await _companyRepository.GetAllCompanies();
            var totalCount = allCompanies.Count;
            var pagedCompanies = allCompanies.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            model.companies = pagedCompanies;
            model.states = await _stateRepository.GetAllStates();     // ✅ Working dropdown
            model.country = await _countryRepository.GetAllCountries(); // ✅ Working dropdown
            await SetPagePermissions("Company");
            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.ShowAllButton = totalCount > pageSize;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            var companies = await _companyRepository.GetAllCompanies();
            return Json(companies);
        }
        [HttpPost]  // 🔥 REMOVE [Route("~/Company/Save")]
        public async Task<IActionResult> Save(IFormFile? file, [FromForm] string? companyJson)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔥 SAVE HIT - companyJson: {companyJson?.Substring(0, Math.Min(100, companyJson.Length))}");

                string path = "";
                if (file != null && file.Length > 0)
                {
                    var folderName = Path.Combine(_webHostEnvironment.WebRootPath, "Companys/Images");
                    Directory.CreateDirectory(folderName);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    path = $"/Companys/Images/{fileName}";

                    using var stream = new FileStream(Path.Combine(_webHostEnvironment.WebRootPath, path.TrimStart('/')), FileMode.Create);
                    await file.CopyToAsync(stream);
                }

                if (string.IsNullOrEmpty(companyJson))
                    return Json(new { success = false, message = "No data" });

                var company = JsonConvert.DeserializeObject<Company>(companyJson);
                if (company == null)
                    return Json(new { success = false, message = "Bad data" });

                var result = await _companyRepository.Add(company, path);
                System.Diagnostics.Debug.WriteLine($"🔥 SAVE RESULT: {result}");

                return Json(new { success = true, message = result ?? "SUCCESS" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🔥 SAVE ERROR: {ex}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
[Route("~/Company/Edit")]
public async Task<IActionResult> Edit(IFormFile? file, [FromForm] string? companyJson)
{
    try
    {
        System.Diagnostics.Debug.WriteLine($"🔥 EDIT HIT - companyJson: {companyJson?.Substring(0, 100)}");

        string path = "";
        if (file != null && file.Length > 0)
        {
            var folderName = Path.Combine(_webHostEnvironment.WebRootPath, "Companys/Images");
            Directory.CreateDirectory(folderName);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            path = $"/Companys/Images/{fileName}";
            
            using var stream = new FileStream(Path.Combine(_webHostEnvironment.WebRootPath, path.TrimStart('/')), FileMode.Create);
            await file.CopyToAsync(stream);
        }

        if (string.IsNullOrEmpty(companyJson))
            return Json(new { success = false, message = "No data" });

        var company = JsonConvert.DeserializeObject<Company>(companyJson);
        if (company == null)
            return Json(new { success = false, message = "Bad data" });

        var success = await _companyRepository.Edit(company, path);
        return Json(new { success, message = success ? "SUCCESS" : "FAILED" });
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"🔥 EDIT ERROR: {ex}");
        return Json(new { success = false, message = ex.Message });
    }
}
        [HttpGet]
        public async Task<JsonResult> GetCompany(string companyID)
        {
            try
            {
                if (int.TryParse(companyID, out int id))
                {
                    var company = await _companyRepository.GetById(id);
                    if (company != null)
                    {
                        // 🔥 FIX 1: Ensure CompanyID (uppercase) exists
                        if (company.CompanyID == 0) company.CompanyID = id;

                        // 🔥 FIX 2: Map ALL Comp_* fields (this was missing!)
                        company.Comp_Name = company.Comp_Name ?? "";
                        company.Comp_Address1 = company.Comp_Address1 ?? "";
                        company.Comp_Address2 = company.Comp_Address2 ?? "";
                        company.Comp_Area = company.Comp_Area ?? "";
                        company.Comp_City = company.Comp_City ?? "";
                        company.Comp_Pincode = company.Comp_Pincode ?? "";
                        company.Comp_Contact_No = company.Comp_Contact_No ?? "";
                        company.Comp_Email = company.Comp_Email ?? "";
                        company.Comp_PAN_No = company.Comp_PAN_No ?? "";
                        company.Comp_TAN_No = company.Comp_TAN_No ?? "";
                        company.Comp_VAT_No = company.Comp_VAT_No ?? "";
                        company.Comp_GST_No = company.Comp_GST_No ?? "";
                        company.Comp_TIN_No = company.Comp_TIN_No ?? "";
                        company.Comp_PTTax_No = company.Comp_PTTax_No ?? "";
                        company.SMTP_Server_Name = company.SMTP_Server_Name ?? "";
                        company.SMTP_Port_No = company.SMTP_Port_No ?? "";
                        company.SMTP_User_Name = company.SMTP_User_Name ?? "";
                        company.SMTP_User_Password = company.SMTP_User_Password ?? "";

                        // 🔥 FIX 3: Debug log
                        System.Diagnostics.Debug.WriteLine($"🔥 CompanyID={company.CompanyID}, Name={company.Comp_Name}");

                        return Json(company);
                    }
                }
                return Json(new { error = "Company not found" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        [HttpGet]
        public async Task<JsonResult> DeleteCompany(string companyId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🗑️ DELETE CALLED: {companyId}");

                if (!int.TryParse(companyId, out int id))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Invalid ID");
                    return Json(new { success = false, message = "Invalid Company ID" });
                }

                // 🔥 Get company for image path
                var company = await _companyRepository.GetById(id);
                string? imagePath = company?.PicturePath;

                // 🔥 Delete from database
                bool dbSuccess = await _companyRepository.Delete(id);

                // 🔥 Delete image file if exists
                if (dbSuccess && !string.IsNullOrEmpty(imagePath))
                {
                    try
                    {
                        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                            System.Diagnostics.Debug.WriteLine($"🖼️ IMAGE DELETED: {fullPath}");
                        }
                    }
                    catch (Exception imgEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"🖼️ IMAGE DELETE ERROR: {imgEx.Message}");
                    }
                }

                var result = new
                {
                    success = dbSuccess,
                    message = dbSuccess ? "Company deleted successfully!" : "Failed to delete company",
                    imageDeleted = !string.IsNullOrEmpty(imagePath)
                };

                System.Diagnostics.Debug.WriteLine($"🗑️ FINAL RESULT: {JsonConvert.SerializeObject(result)}");
                return Json(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🗑️ CONTROLLER ERROR: {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // 🔥 NEW HELPER METHOD - Add this to controller
        private async Task DeleteImageFile(string imagePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath) && imagePath.StartsWith("/Companys/Images/"))
                {
                    var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                        System.Diagnostics.Debug.WriteLine($"🗑️ DELETED IMAGE: {fullPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🗑️ IMAGE DELETE ERROR: {ex.Message}");
            }
        }
    }
}