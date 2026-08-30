namespace StudentRecordSystem.Models;

/// <summary>
/// Represents a full student academic record: personal details, course
/// enrolment, current status, and the list of units they are/were enrolled
/// in. WAM and GPA are calculated automatically from completed unit marks.
/// </summary>
public class Student
{
    // --- Personal details ---
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string EmergencyContact { get; set; } = string.Empty;

    // --- Course & status ---
    public string CourseCode { get; set; } = string.Empty;
    public StudentStatus Status { get; set; } = StudentStatus.Active;
    public DateTime EnrollmentDate { get; set; } = DateTime.Today;

    // --- Academic record ---
    public List<UnitRecord> UnitRecords { get; set; } = new();

    public List<UnitRecord> CurrentUnits => UnitRecords.Where(u => !u.IsCompleted).ToList();

    public List<UnitRecord> CompletedUnits => UnitRecords.Where(u => u.IsCompleted).ToList();

    public int TotalCreditPointsCompleted => CompletedUnits.Sum(u => u.CreditPoints);

    /// <summary>
    /// Weighted Average Mark: the credit-point-weighted average of all
    /// completed unit marks. Returns 0 if no units have been completed yet.
    /// </summary>
    public double CalculateWAM()
    {
        List<UnitRecord> completed = CompletedUnits;
        if (completed.Count == 0)
        {
            return 0;
        }

        double totalWeightedMarks = completed.Sum(u => u.Mark!.Value * u.CreditPoints);
        double totalCredits = completed.Sum(u => u.CreditPoints);
        return totalCredits == 0 ? 0 : totalWeightedMarks / totalCredits;
    }

    /// <summary>
    /// Grade Point Average on a 7-point scale, weighted by credit points.
    /// Returns 0 if no units have been completed yet.
    /// </summary>
    public double CalculateGPA()
    {
        List<UnitRecord> completed = CompletedUnits;
        if (completed.Count == 0)
        {
            return 0;
        }

        double totalWeightedPoints = completed.Sum(u => GradeCalculator.GetGradePoint(u.Grade) * u.CreditPoints);
        double totalCredits = completed.Sum(u => u.CreditPoints);
        return totalCredits == 0 ? 0 : totalWeightedPoints / totalCredits;
    }
}
