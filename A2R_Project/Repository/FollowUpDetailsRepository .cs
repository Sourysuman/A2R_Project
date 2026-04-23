using A2R_Project.Context;
using A2R_Project.Interface;
using A2R_Project.Models;
using Dapper;
using System.Data;

namespace A2R_Project.Repositories
{
    public class FollowUpRepository : IFollowUpRepository
    {
        private readonly AppDbContext _dbContext;

        public FollowUpRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> UpdateFollowUp(FollowUpDetail followUpDetail)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@StudentInquiryID", followUpDetail.StudentInquiryID);
                parameters.Add("@FollowUpDesc1", followUpDetail.FollowUpDesc1);
                parameters.Add("@FollowUpDate1", followUpDetail.FollowUpDate1);
                parameters.Add("@FollowUpDesc2", followUpDetail.FollowUpDesc2);
                parameters.Add("@FollowUpDate2", followUpDetail.FollowUpDate2);

                var rowsAffected = await connection.ExecuteAsync(
                    "sp_AddFollowUpDetails",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // Log exception if you have logger
                Console.WriteLine($"FollowUp Update Error: {ex.Message}");
                return false;
            }
        }
    }
}