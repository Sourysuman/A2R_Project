namespace A2R_Project.Models
{
   
        public class Course
        {
            public int CourseID { get; set; }
            public string? CourseName { get; set; }
            public List<string>? CourseNames { get; set; }
            public int? IsActive { get; set; }
            public int? IsDeleted { get; set; }
        }
    }

