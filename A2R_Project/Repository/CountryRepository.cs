using A2R_Project.Context;
using A2R_Project.Interface;
using A2R_Project.Models;
using Dapper;
using System.Data;

namespace A2R_Project.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly AppDbContext _dbContext;

        public CountryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var countries = await connection.QueryAsync<Country>(
                    "sp_GetAllCountries",
                    commandType: CommandType.StoredProcedure
                );
                return countries.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> SaveCountry(Country country)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("CountryName", country.CountryName);
                parameters.Add("IsDeleted", country.IsDeleted ?? 0);
                parameters.Add("IsActive", country.IsActive ?? 0);
                parameters.Add("ResultMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

                await connection.ExecuteAsync("sp_AddCountry", parameters, commandType: CommandType.StoredProcedure);

                string resultMessage = parameters.Get<string>("ResultMessage")?.Trim() ?? "Error";
                return resultMessage;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving country: {ex.Message}");
            }
        }

        public async Task<string> UpdateCountries(Country country)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("CountryID", country.CountryID);
                parameters.Add("CountryName", country.CountryName);
                parameters.Add("IsDeleted", country.IsDeleted ?? 0);
                parameters.Add("IsActive", country.IsActive ?? 0);

                int rowsAffected = await connection.ExecuteAsync(
                    "sp_EditCountry",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return rowsAffected > 0 ? "success" : "failed";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating country: {ex.Message}");
            }
        }

        public async Task<Country> GetCountries(string countryId)
        {
            try
            {
                if (int.TryParse(countryId, out int id))
                {
                    using var connection = _dbContext.CreateConnection();
                    var country = await connection.QuerySingleOrDefaultAsync<Country>(
                        "sp_ViewCountry",
                        new { CountryID = id },
                        commandType: CommandType.StoredProcedure
                    );
                    return country;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting country: {ex.Message}");
            }
        }

        public async Task<string> DeleteCountry(string countryId)
        {
            try
            {
                if (int.TryParse(countryId, out int id))
                {
                    using var connection = _dbContext.CreateConnection();
                    int rowsAffected = await connection.ExecuteAsync(
                        "sp_DeleteCountry",
                        new { CountryID = id },
                        commandType: CommandType.StoredProcedure
                    );

                    return rowsAffected > 0 ? "success" : "failed";
                }
                return "failed";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting country: {ex.Message}");
            }
        }
    }
}