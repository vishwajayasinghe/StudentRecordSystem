namespace StudentRecordSystem.Models;

/// <summary>
/// Converts numeric marks to letter grades and grade points, using a
/// standard 4.0 GPA scale (HD=4.0, D=3.0, C=2.0, P=1.0, N=0.0).
/// </summary>
public static class GradeCalculator
{
    public static string GetGrade(double mark) => mark switch
    {
        >= 80 => "HD",
        >= 70 => "D",
        >= 60 => "C",
        >= 50 => "P",
        _ => "N"
    };

    public static double GetGradePoint(string grade) => grade switch
    {
        "HD" => 4.0,
        "D" => 3.0,
        "C" => 2.0,
        "P" => 1.0,
        _ => 0.0
    };
}
