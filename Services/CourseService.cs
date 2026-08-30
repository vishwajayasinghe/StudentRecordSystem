using StudentRecordSystem.Models;

namespace StudentRecordSystem.Services;

/// <summary>
/// Provides the list of courses/programs students can be enrolled in.
/// Kept as a small seeded list rather than a fully editable table to keep
/// the assignment's scope manageable — see README for how to extend this.
/// </summary>
public class CourseService
{
    private readonly List<Course> _courses = new()
    {
        new Course { Code = "BCyberSec", Name = "Bachelor of Cyber Security", Faculty = "Science", DurationYears = 3 },
        new Course { Code = "BCompSci", Name = "Bachelor of Computer Science", Faculty = "Science", DurationYears = 3 },
        new Course { Code = "BSoftEng", Name = "Bachelor of Software Engineering", Faculty = "Science", DurationYears = 4 },
    };

    public List<Course> GetAll() => _courses;

    public Course? GetByCode(string code) => _courses.FirstOrDefault(c => c.Code == code);
}
