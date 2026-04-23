using A2R_Project.Models;

namespace A2R_Project.Repositories.Interfaces
{
    public interface IBranchRepository
    {
        Task<List<Branch>> GetAllBranches();
        Task<string> AddBranch(Branch branch);
        Task<bool> UpdateBranch(Branch branch);
        Task<Branch> GetBranchById(int branchId);
        Task<bool> DeleteBranch(int branchId);
    }
}