using A2R_Project.Models;

namespace A2R_Project.Interface
{
    public interface ICountryRepository
    {
        Task<List<Country>> GetAllCountries();
        Task<string> SaveCountry(Country country);
        Task<string> UpdateCountries(Country country);
        Task<Country> GetCountries(string countryId);
        Task<string> DeleteCountry(string countryId);
    }
}
