using A2R_Project.Context;
using A2R_Project.Interface;
using A2R_Project.Models;
using Dapper;
using System.Data;

namespace A2R_Project.Repositories
{
    public class CompanyRepository : ICompanyRepository  // ← MISSING COLON ':'
    {
        private readonly AppDbContext _dbContext;  // ← MISSING OPENING BRACE '{'

        public CompanyRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Company>> GetAllCompanies()
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var companies = await connection.QueryAsync<Company>(
                    "sp_GetAllCompanies",
                    commandType: CommandType.StoredProcedure
                );
                return companies.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> Add(Company company, string picturePath)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("Comp_Name", company.Comp_Name);
                parameters.Add("Comp_Address1", company.Comp_Address1);
                parameters.Add("Comp_Address2", company.Comp_Address2);
                parameters.Add("Comp_Area", company.Comp_Area);
                parameters.Add("Comp_City", company.Comp_City);
                parameters.Add("Comp_State_ID", company.Comp_State_ID);
                parameters.Add("Comp_Country_ID", company.Comp_Country_ID);
                parameters.Add("Comp_Pincode", company.Comp_Pincode);
                parameters.Add("Comp_PAN_No", company.Comp_PAN_No);
                parameters.Add("Comp_TAN_No", company.Comp_TAN_No);
                parameters.Add("Comp_TIN_No", company.Comp_TIN_No);
                parameters.Add("Comp_VAT_No", company.Comp_VAT_No);
                parameters.Add("Comp_GST_No", company.Comp_GST_No);
                parameters.Add("Comp_PTTax_No", company.Comp_PTTax_No);
                parameters.Add("Comp_Email", company.Comp_Email);
                parameters.Add("Comp_Contact_No", company.Comp_Contact_No);
                parameters.Add("SMTP_Server_Name", company.SMTP_Server_Name);
                parameters.Add("SMTP_Port_No", company.SMTP_Port_No);
                parameters.Add("SMTP_User_Name", company.SMTP_User_Name);
                parameters.Add("PicturePath", picturePath);  // 🔥 FIXED POSITION
                parameters.Add("SMTP_User_Password", company.SMTP_User_Password);
                parameters.Add("IsActive", company.IsActive);
                parameters.Add("ResultMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

                await connection.ExecuteAsync("sp_AddCompany", parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<string>("ResultMessage")?.Trim() ?? "SUCCESS";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ADD ERROR: {ex}");
                throw;
            }
        }

        public async Task<bool> Edit(Company company, string picturePath)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("CompanyID", company.CompanyID);
                parameters.Add("Comp_Name", company.CompanyName ?? company.Comp_Name);
                parameters.Add("Comp_Address1", company.Address1 ?? company.Comp_Address1);
                parameters.Add("Comp_Address2", company.Address2 ?? company.Comp_Address2);
                parameters.Add("Comp_Area", company.Area ?? company.Comp_Area);
                parameters.Add("Comp_City", company.City ?? company.Comp_City);
                parameters.Add("Comp_State_ID", company.Comp_State_ID);
                parameters.Add("Comp_Country_ID", company.Comp_Country_ID);
                parameters.Add("Comp_Pincode", company.Pincode ?? company.Comp_Pincode);
                parameters.Add("Comp_PAN_No", company.PANNo ?? company.Comp_PAN_No);
                parameters.Add("Comp_TAN_No", company.TANNo ?? company.Comp_TAN_No);
                parameters.Add("Comp_TIN_No", company.TINNo ?? company.Comp_TIN_No);
                parameters.Add("Comp_VAT_No", company.VATNo ?? company.Comp_VAT_No);
                parameters.Add("Comp_GST_No", company.GSTNo ?? company.Comp_GST_No);
                parameters.Add("Comp_PTTax_No", company.PTTaxNo ?? company.Comp_PTTax_No);
                parameters.Add("Comp_Email", company.Email ?? company.Comp_Email);
                parameters.Add("Comp_Contact_No", company.ContactNo ?? company.Comp_Contact_No);
                parameters.Add("SMTP_Server_Name", company.SMTPServerName ?? company.SMTP_Server_Name);
                parameters.Add("SMTP_Port_No", company.SMTPPortNo ?? company.SMTP_Port_No);
                parameters.Add("SMTP_User_Name", company.SMTPUserName ?? company.SMTP_User_Name);
                parameters.Add("SMTP_User_Password", company.SMTPUserPassword ?? company.SMTP_User_Password);
                parameters.Add("Comp_CIN_No", company.CINNo ?? company.Comp_CIN_No);
                parameters.Add("PicturePath", picturePath);
                parameters.Add("IsActive", company.IsActive);

                int rowAffected = await connection.ExecuteAsync("sp_EditCompany", parameters, commandType: CommandType.StoredProcedure);
                return rowAffected > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Company?> GetById(int companyId)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var company = await connection.QuerySingleOrDefaultAsync<Company>(
                    "sp_ViewCompany",
                    new { companyId },
                    commandType: CommandType.StoredProcedure
                );

                System.Diagnostics.Debug.WriteLine($"🔥 Repository GetById: ID={companyId}, Company={company?.Comp_Name}");
                return company;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🔥 Repository GetById ERROR: {ex}");
                throw;
            }
        }
        public async Task<bool> Delete(int companyId)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                connection.Open();  // 🔥 Synchronous open

                var result = await connection.ExecuteAsync(
                    "sp_DeleteCompany",
                    new { companyId },
                    commandType: CommandType.StoredProcedure
                );

                System.Diagnostics.Debug.WriteLine($"🗑️ DELETE SUCCESS: Rows={result}");
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🗑️ ERROR: {ex}");
                return false;
            }
        }
    }
    }