using A2R_Project.Context;
using A2R_Project.Interfaces;
using A2R_Project.Models;
using Dapper;
using System.Data;

namespace A2R_Project.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly AppDbContext _dbContext;

        public LoginRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AdminLogin>> GetAllAdminLogin()
        {
            using var connection = _dbContext.CreateConnection();
            var adminLogin = await connection.QueryAsync<AdminLogin>("sp_GetAllAdminLogin", commandType: CommandType.StoredProcedure);
            return adminLogin.ToList();
        }

        public async Task<string> Add(AdminLogin adminLogin)
        {
            using var connection = _dbContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@LoginID", 0);
            parameters.Add("@UserName", adminLogin.UserName);
            parameters.Add("@Password", adminLogin.Password);
            parameters.Add("@Role_ID", adminLogin.Role_ID);
            parameters.Add("@IsActive", adminLogin.IsActive);
            parameters.Add("@ResultMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

            var result = await connection.ExecuteAsync("sp_AddAdminLogin", parameters, commandType: CommandType.StoredProcedure);
            var message = parameters.Get<string>("@ResultMessage")?.Trim() ?? "Error";

            Console.WriteLine($"Add result: {message}, rows: {result}");
            return message;
        }

        public async Task<bool> Edit(AdminLogin adminLogin)
        {
            using var connection = _dbContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@LoginID", adminLogin.LoginID);
            parameters.Add("@UserName", adminLogin.UserName);
            parameters.Add("@Role_ID", adminLogin.Role_ID);
            parameters.Add("@Password", adminLogin.Password ?? "");
            parameters.Add("@IsActive", adminLogin.IsActive);

            int rowsAffected = await connection.ExecuteAsync("sp_EditAdminLogin", parameters, commandType: CommandType.StoredProcedure);
            Console.WriteLine($"Edit rows affected: {rowsAffected}");
            return rowsAffected > 0;
        }

        public async Task<AdminLogin> GetById(int LoginID)
        {
            using var connection = _dbContext.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<AdminLogin>("sp_ViewAdminLogin", new { LoginID }, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> Delete(int LoginID)
        {
            using var connection = _dbContext.CreateConnection();
            int rowAffected = await connection.ExecuteAsync("sp_DeleteAdminLogin", new { LoginID }, commandType: CommandType.StoredProcedure);
            return rowAffected > 0;
        }

        public async Task<AdminLogin> GetLoginUser(string userName, string password)
        {
            using var connection = _dbContext.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<AdminLogin>("sp_LoginUser", new { userName, password }, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<AdminLogin>> GetUsersByRole(string roleName)
        {
            using var connection = _dbContext.CreateConnection();
            var users = await connection.QueryAsync<AdminLogin>("sp_GetUsersByRole", new { RoleName = roleName }, commandType: CommandType.StoredProcedure);
            return users.ToList();
        }

        public async Task<AdminLogin> FindByEmailAsync(string email)
        {
            using var connection = _dbContext.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<AdminLogin>("sp_FindUserByEmail",
                new { Email = email }, commandType: CommandType.StoredProcedure);
        }
        // ✅ All methods using your Admin_Login table
        public async Task<AdminLogin> FindByUsernameAsync(string username)
        {
            using var connection = _dbContext.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<AdminLogin>(
                @"SELECT LoginID, UserName, Password, Role_ID, IsActive, IsDeleted, Email, Notes 
          FROM Admin_Login 
          WHERE UserName = @UserName AND IsDeleted = 0",
                new { UserName = username }
            );
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string username)
        {
            using var connection = _dbContext.CreateConnection();

            // ✅ SUPER SAFE: Use GUID + Random
            var token = Guid.NewGuid().ToString("N").Substring(0, 32); // Always 32 chars!
            var expiryDate = DateTime.UtcNow.AddMinutes(15);

            int rows = await connection.ExecuteAsync(@"
        UPDATE Admin_Login 
        SET ResetToken = @Token, ResetTokenExpiry = @ExpiryDate 
        WHERE UserName = @Username AND IsDeleted = 0",
                new { Username = username, Token = token, ExpiryDate = expiryDate });

            Console.WriteLine($"✅ Token '{token}' generated for '{username}' (rows: {rows})");
            return rows > 0 ? token : null;
        }

        public async Task<bool> ResetPasswordAsync(string username, string token, string newPassword)
        {
            using var connection = _dbContext.CreateConnection();

            int rows = await connection.ExecuteScalarAsync<int>(@"
        IF EXISTS (
            SELECT 1 FROM Admin_Login 
            WHERE UserName = @Username 
            AND ResetToken = @Token 
            AND ResetTokenExpiry > GETUTCDATE()
            AND IsDeleted = 0
        )
        BEGIN
            UPDATE Admin_Login 
            SET Password = @NewPassword, 
                ResetToken = NULL, 
                ResetTokenExpiry = NULL
            WHERE UserName = @Username;
            SELECT @@ROWCOUNT;
        END
        ELSE SELECT 0;",
                new { Username = username, Token = token, NewPassword = newPassword });

            Console.WriteLine($"Reset rows: {rows} for {username}");
            return rows > 0;
        }
    }
}