using A2R_Project.Context;
using A2R_Project.Models;
using A2RSystemInterface;
using Dapper;
using System.Data;

namespace A2R_Project.Repository
{
    public class AccessControlRepository : IAccessControl
    {
        private readonly AppDbContext _dbContext;

        public AccessControlRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<AccessControl>> GetAllAccessControls()
        {
            using var connection = _dbContext.CreateConnection();
            return (await connection.QueryAsync<AccessControl>(
                "sp_FilterAccessControls",  // ✅ USE SAME SP with NULL params
                new { RoleName = (string)null, UserName = (string)null },
                commandType: CommandType.StoredProcedure)).AsList();
        }
        public async Task<List<AccessControl>> GetFilteredAccessControls(string? roleName, string? userName)
        {
            using var connection = _dbContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@RoleName", roleName ?? (object)DBNull.Value);
            parameters.Add("@UserName", userName ?? (object)DBNull.Value);

            return (await connection.QueryAsync<AccessControl>(
                "sp_FilterAccessControls", parameters,
                commandType: CommandType.StoredProcedure)).AsList();
        }
        // ✅ UPDATE - Dapper auto-opens, manual transaction
        public async Task<string> UpdateAccessControl(List<AccessControl> accessControls)
        {
            using var connection = _dbContext.CreateConnection();

            foreach (var item in accessControls)
            {
                var parameters = new DynamicParameters();
                parameters.Add("AccessID", item.AccessID);
                parameters.Add("Module_Flag", item.Module_Flag);
                parameters.Add("Insert_Flag", item.Insert_Flag);
                parameters.Add("Update_Flag", item.Update_Flag);
                parameters.Add("Delete_Flag", item.Delete_Flag);

                int rowsAffected = await connection.ExecuteAsync(
                    "sp_UpdateAccessControl",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected == 0)
                {
                    return $"No rows updated for AccessID: {item.AccessID}";
                }
            }

            return "Access has been modified successfully.";
        }

        // ✅ INSERT - Dapper auto-opens connection
        public async Task<string> InsertAccessControlValues(AccessControl request)
        {
            using var connection = _dbContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("RoleName", request.RoleName ?? "");
            parameters.Add("UserName", request.UserName ?? "");
            parameters.Add("MenuName", request.MenuName ?? "");
            parameters.Add("MenuIcon", request.MenuIcon ?? "");
            parameters.Add("SubMenuName", request.SubMenuName ?? "");
            parameters.Add("SubMenuController", request.SubMenuController ?? "");
            parameters.Add("SubMenuAction", request.SubMenuAction ?? "");
            parameters.Add("SubMenuChildAction", request.SubMenuChildAction ?? "");
            parameters.Add("InsertFlag", request.Insert_Flag);
            parameters.Add("UpdateFlag", request.Update_Flag);
            parameters.Add("DeleteFlag", request.Delete_Flag);
            parameters.Add("ModuleFlag", request.Module_Flag);
            parameters.Add("IconClass", request.IconClass ?? "");

            int rowsAffected = await connection.ExecuteAsync(
                "sp_InsertAccessControlValues",
                parameters,
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0 ? "successfully" : "failed";
        }
    }
}