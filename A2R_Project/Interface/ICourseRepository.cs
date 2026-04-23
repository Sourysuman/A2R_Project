using A2R_Project.Models;


namespace A2RSystemWebApp.Interfaces
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetAllCourses();
        Task<string> Add(Course course);
        Task<bool> Edit(Course course);
        Task<Course> GetById(int courseID);
        Task<bool> Delete(int courseID);
        Task<List<string>> GetCourseNamesByIds(List<string> courseIds);
    }
}