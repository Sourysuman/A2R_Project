using A2R_Project.Models;

namespace A2R_Project.Interface
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetAllCompanies();
        Task<string> Add(Company company, string picturePath);
        Task<bool> Edit(Company company, string picturePath);
        Task<Company?> GetById(int companyId);
        Task<bool> Delete(int companyId);
    }
}