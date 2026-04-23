using A2R_Project.Context;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using Dapper;
using System.Data;

namespace A2R_Project.Repositories
{
    public class StateRepository : IStateRepository
    {
        private readonly AppDbContext _dbContext;

        public StateRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<State>> GetAllStates()
        {
            using var connection = _dbContext.CreateConnection();
            var states = await connection.QueryAsync<State>("sp_GetAllStates", commandType: CommandType.StoredProcedure);
            return states.ToList();
        }

        public async Task<string> Add(State state)
        {
            using var connection = _dbContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("CountryID", state.CountryID);
            parameters.Add("StateCode", state.StateCode);
            parameters.Add("StateName", state.StateName);
            parameters.Add("IsActive", state.IsActive);
            parameters.Add("IsDeleted", state.IsDeleted);

            parameters.Add("ResultMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

            await connection.ExecuteAsync("sp_AddState", parameters, commandType: CommandType.StoredProcedure);

            string resultMessage = parameters.Get<string>("ResultMessage")?.Trim() ?? "success";
            return resultMessage;
        }

        public async Task<bool> Edit(State state)
        {
            using var connection = _dbContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("StateID", state.StateID);
            parameters.Add("CountryID", state.CountryID);
            parameters.Add("StateCode", state.StateCode);
            parameters.Add("StateName", state.StateName);
            parameters.Add("IsActive", state.IsActive);
            parameters.Add("IsDeleted", state.IsDeleted);

            int rowAffected = await connection.ExecuteAsync("sp_EditState", parameters, commandType: CommandType.StoredProcedure);
            return rowAffected > 0;
        }

        public async Task<State> GetById(int stateID)
        {
            using var connection = _dbContext.CreateConnection();
            var state = await connection.QuerySingleOrDefaultAsync<State>("sp_ViewState",
                new { StateID = stateID }, commandType: CommandType.StoredProcedure);
            return state ?? new State();
        }

        public async Task<bool> Delete(int stateID)
        {
            using var connection = _dbContext.CreateConnection();
            int rowAffected = await connection.ExecuteAsync("sp_DeleteState",
                new { StateID = stateID }, commandType: CommandType.StoredProcedure);
            return rowAffected > 0;
        }

        public async Task<List<State>> GetAllCountries()
        {
            using var connection = _dbContext.CreateConnection();
            // Use stored procedure or raw query for countries
            var countries = await connection.QueryAsync<State>("sp_GetAllCountries",
                commandType: CommandType.StoredProcedure);
            return countries.ToList();
        }
    }
}