using A2R_Project.Context;
using A2R_Project.Models;
using A2R_Project.Repositories.Interfaces;
using Dapper;
using System.Data;

namespace A2R_Project.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly AppDbContext _dbContext;

        public BranchRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Branch>> GetAllBranches()
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var branches = await connection.QueryAsync<Branch>("sp_GetAllBranches", commandType: CommandType.StoredProcedure);
                return branches.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> AddBranch(Branch branch)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var parameters = new DynamicParameters();

                // ✅ CORRECT FIELD MAPPING
                parameters.Add("CompanyID", branch.CompanyID);
                parameters.Add("BranchName", branch.BranchName);
                parameters.Add("BranchAddress1", branch.BranchAddress1 ?? branch.Address1);
                parameters.Add("BranchCity", branch.City ?? branch.BranchCity);  // ✅ FIXED
                parameters.Add("BranchStateId", branch.BranchStateId);
                parameters.Add("BranchCountryId", branch.BranchCountryId);
                parameters.Add("BranchPincode", branch.Pincode);
                parameters.Add("BranchPanNo", branch.PANNo);
                parameters.Add("BranchGstNo", branch.GSTNo);
                parameters.Add("BranchTanNo", branch.TANNo);
                parameters.Add("BranchVatNo", branch.VATNo);
                parameters.Add("BranchPttaxNo", branch.PTTaxNo);
                parameters.Add("BranchEmail", branch.Email);
                parameters.Add("BranchContactNo", branch.ContactNo);
                parameters.Add("IsActive", branch.IsActive ?? 1);

                parameters.Add("ResultMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

                await connection.ExecuteAsync("sp_AddBranch", parameters, commandType: CommandType.StoredProcedure);
                string resultMessage = parameters.Get<string>("ResultMessage")?.Trim() ?? "Error Occurred";
                return resultMessage;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public async Task<bool> UpdateBranch(Branch branch)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var parameters = new DynamicParameters();

                // ✅ CORRECT FIELD MAPPING
                parameters.Add("BranchId", branch.BranchId);
                parameters.Add("CompanyID", branch.CompanyID);
                parameters.Add("BranchName", branch.BranchName);
                parameters.Add("BranchAddress1", branch.BranchAddress1 ?? branch.Address1);
                parameters.Add("BranchCity", branch.City ?? branch.BranchCity);  // ✅ FIXED
                parameters.Add("BranchStateId", branch.BranchStateId);
                parameters.Add("BranchCountryId", branch.BranchCountryId);
                parameters.Add("BranchPincode", branch.Pincode);
                parameters.Add("BranchPanNo", branch.PANNo);
                parameters.Add("BranchGstNo", branch.GSTNo);
                parameters.Add("BranchTanNo", branch.TANNo);
                parameters.Add("BranchVatNo", branch.VATNo);
                parameters.Add("BranchPttaxNo", branch.PTTaxNo);
                parameters.Add("BranchEmail", branch.Email);
                parameters.Add("BranchContactNo", branch.ContactNo);
                parameters.Add("IsActive", branch.IsActive);

                int rowAffected = await connection.ExecuteAsync("sp_EditBranch", parameters, commandType: CommandType.StoredProcedure);
                return rowAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update Error: {ex.Message}");
                return false;
            }
        }

        public async Task<Branch> GetBranchById(int branchId)
        {
            using var connection = _dbContext.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Branch>("sp_ViewBranch",
                new { branchId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> DeleteBranch(int branchId)
        {
            using var connection = _dbContext.CreateConnection();
            int rows = await connection.ExecuteAsync("sp_DeleteBranch",
                new { branchId }, commandType: CommandType.StoredProcedure);
            return rows > 0;
        }
    }
}