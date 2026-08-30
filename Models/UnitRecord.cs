namespace StudentRecordSystem.Models;

/// <summary>
/// Represents a single unit a student is (or was) enrolled in.
/// If Mark is null, the unit is treated as a current enrolment (not yet graded).
/// If Mark has a value, it is treated as a completed result.
/// </summary>
public class UnitRecord
{
    public string UnitCode { get; set; } = "";
    public string UnitName { get; set; } = "";
    public string Semester { get; set; } = "";
    public int CreditPoints { get; set; } = 15;
    public double? Mark { get; set; }

    public bool IsCompleted => Mark.HasValue;

    public string Grade => Mark.HasValue ? GradeCalculator.GetGrade(Mark.Value) : "-";
}
