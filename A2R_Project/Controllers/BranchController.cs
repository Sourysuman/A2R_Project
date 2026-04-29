using A2R_Project.Interface;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using A2R_Project.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace A2R_Project.Controllers
{
    public class BranchController : BaseController
    {
        private readonly IBranchRepository _branchRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IStateRepository _stateRepository;
        private readonly ICountryRepository _countryRepository;

        public BranchController(IBranchRepository branchRepository, ICompanyRepository companyRepository,
            IStateRepository stateRepository, ICountryRepository countryRepository)
        {
            _branchRepository = branchRepository;
            _companyRepository = companyRepository;
            _stateRepository = stateRepository;
            _countryRepository = countryRepository;
        }

        public async Task<IActionResult> List(int page = 1, int pageSize = 10)
        {
            var model = new BranchViewModel();
         
            try
            {
                var allBranches = await _branchRepository.GetAllBranches() ?? new List<Branch>();
                var totalCount = allBranches.Count;
                var pagedBranches = allBranches.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                model.branches = pagedBranches;
                model.companies = await _companyRepository.GetAllCompanies() ?? new List<Company>();
                model.states = await _stateRepository.GetAllStates() ?? new List<State>();
                model.country = await _countryRepository.GetAllCountries() ?? new List<Country>();
            }
            catch (Exception ex)
            {
            
                Console.WriteLine($"List Error: {ex.Message}");
                model.branches = new List<Branch>(); 
            }
            await SetPagePermissions("Branch");
            ViewBag.TotalCount = model.branches.Count;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.ShowAllButton = model.branches.Count > pageSize;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            var branches = await _branchRepository.GetAllBranches();
            return Json(branches);
        }


        [HttpPost]
        public async Task<JsonResult> Save([FromBody] Branch branchData)
        {
            try
            {
                if (branchData?.CompanyID == 0 || string.IsNullOrWhiteSpace(branchData?.BranchName))
                    return Json("Company and Branch Name required");

                var response = await _branchRepository.AddBranch(branchData);
                return Json(response ?? "Error occurred");
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Edit([FromBody] Branch branchData)
        {
            try
            {
                if (branchData?.BranchId == 0)
                    return Json("Invalid Branch ID");

                var success = await _branchRepository.UpdateBranch(branchData);
                return Json(success ? "success" : "Update failed");
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message);
            }
        }

        // 🔥 FIXED GetBranch - MAIN FIX
        [HttpGet]
        public async Task<JsonResult> GetBranch(int branchId)
        {
            try
            {
                Console.WriteLine($"🔍 GetBranch called: branchId={branchId}");
                var branch = await _branchRepository.GetBranchById(branchId);
                Console.WriteLine($"📤 Branch result: {branch?.BranchName ?? "NULL"}");

                if (branch == null)
                    return Json(new { error = "Branch not found", BranchId = 0 });

                return Json(branch);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ GetBranch ERROR: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> DeleteBranch(int branchId)
        {
            try
            {
                Console.WriteLine($"🗑️ DeleteBranch: {branchId}");
                var success = await _branchRepository.DeleteBranch(branchId);
                return Json(success ? "success" : "failed");
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message);
            }
        }
    }
}