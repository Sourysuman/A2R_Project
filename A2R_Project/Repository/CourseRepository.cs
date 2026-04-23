using A2R_Project.Context;
using A2R_Project.Models;

using A2RSystemWebApp.Interfaces;
using Dapper;
using System.Data;

namespace A2RSystemWebApp.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _dbContext;

        public CourseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Course>> GetAllCourses()
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var courses = await connection.QueryAsync<Course>("sp_GetAllCourses", commandType: CommandType.StoredProcedure);
                return courses.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> Add(Course course)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("CourseName", course.CourseName);
                parameters.Add("IsActive", course.IsActive);
                parameters.Add("ResultMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

                await connection.ExecuteAsync("sp_AddCourse", parameters, commandType: CommandType.StoredProcedure);
                string resultMessage = parameters.Get<string>("ResultMessage")?.Trim() ?? "Success";

                return resultMessage;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Edit(Course course)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("CourseID", course.CourseID);
                parameters.Add("CourseName", course.CourseName);
                parameters.Add("IsActive", course.IsActive);

                await connection.ExecuteAsync("sp_EditCourse", parameters, commandType: CommandType.StoredProcedure);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Course> GetById(int courseID)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                var course = await connection.QuerySingleOrDefaultAsync<Course>(
                    "sp_ViewCourse",
                    new { CourseID = courseID },
                    commandType: CommandType.StoredProcedure
                );
                return course ?? new Course { CourseID = courseID, CourseName = "Course Not Found", IsActive = 0 };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Delete(int courseID)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                int rowsAffected = await connection.ExecuteAsync(
                    "sp_DeleteCourse",
                    new { CourseID = courseID },
                    commandType: CommandType.StoredProcedure
                );
                return rowsAffected > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<string>> GetCourseNamesByIds(List<string> courseIds)
        {
            try
            {
                using var connection = _dbContext.CreateConnection();
                string courseIdsString = string.Join(",", courseIds);
                var courseNames = await connection.QueryAsync<string>(
                    "sp_GetCourseNamesByIds",
                    new { CourseIds = courseIdsString },
                    commandType: CommandType.StoredProcedure
                );
                return courseNames.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}