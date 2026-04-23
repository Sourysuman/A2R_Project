using A2R_Project.Context;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using Dapper;
using System.Data;

namespace A2R_Project.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _dbContext;

        public RoleRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Role>> GetAllRoles()
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var roles = await connection.QueryAsync<Role>("sp_GetAllRoles", commandType: CommandType.StoredProcedure);
                return roles.ToList();
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> Add(Role role)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();

                // Direct INSERT - NO stored proc needed
                var sql = "INSERT INTO Roles (Role_Name, IsActive, IsDeleted) VALUES (@name, @active, 0)";
                var affected = await connection.ExecuteAsync(sql,
                    new { name = role.Role_Name, active = role.IsActive ?? 1 });

                return affected > 0 ? "success" : "failed";
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("duplicate") ? "duplicate" : "Error";
            }
        }

        public async Task<bool> Edit(Role role)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                int rowAffected = await connection.ExecuteAsync("sp_EditRole", role, commandType: CommandType.StoredProcedure);
                return rowAffected > 0;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Role?> GetById(int roleId)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                return await connection.QuerySingleOrDefaultAsync<Role>("sp_ViewRole",
                    new { Role_ID = roleId }, commandType: CommandType.StoredProcedure);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Delete(int roleId)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                int rowAffected = await connection.ExecuteAsync("sp_DeleteRole",
                    new { Role_ID = roleId }, commandType: CommandType.StoredProcedure);
                return rowAffected > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}