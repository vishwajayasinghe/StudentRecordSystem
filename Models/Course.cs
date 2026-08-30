namespace StudentRecordSystem.Models;

/// <summary>
/// Represents a degree/program that students can be enrolled in.
/// Kept as a small, seeded reference list (see CourseService) rather than a
/// full admin-editable table, to keep the assignment's scope manageable.
/// </summary>
public class Course
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Faculty { get; set; } = "";
    public int DurationYears { get; set; } = 3;
}
