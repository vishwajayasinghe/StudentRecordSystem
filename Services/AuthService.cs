using System.Security.Cryptography;
using System.Text;
using StudentRecordSystem.Models;

namespace StudentRecordSystem.Services;

/// <summary>
/// Handles login and access control for one browser session (Scoped = one
/// instance per connected browser tab). Supports two roles:
///   - Admin: a single hardcoded demo account, checked against a SHA-256
///     password hash (never the plain-text password), with lockout after
///     repeated failed attempts.
///   - Student: logs in with just their Student ID and can only view their
///     own record (no password — acceptable for a read-only demo role).
///
/// NOTE: this is a simplified authentication system appropriate for a
/// university assignment demo. A production system would use a real
/// identity provider, salted password hashing, and persistent account
/// storage rather than a single hardcoded admin account.
/// </summary>
public class AuthService
{
    private const string AdminUsername = "admin";
    private static readonly string AdminPasswordHash = Hash("admin123"); // demo password: admin123

    private const int MaxFailedAttempts = 3;
    private int _failedAttempts;

    private readonly StudentService _studentService;

    public string Role { get; private set; } = "None"; // "None" | "Admin" | "Student"
    public Student? CurrentStudent { get; private set; }
    public bool IsLockedOut => _failedAttempts >= MaxFailedAttempts;

    public AuthService(StudentService studentService)
    {
        _studentService = studentService;
    }

    public (bool Success, string Message) LoginAsAdmin(string username, string password)
    {
        if (IsLockedOut)
        {
            return (false, "Account locked after too many failed attempts. Restart the app to reset (demo only).");
        }

        if (username == AdminUsername && Hash(password) == AdminPasswordHash)
        {
            Role = "Admin";
            CurrentStudent = null;
            _failedAttempts = 0;
            return (true, "Logged in as Administrator.");
        }

        _failedAttempts++;
        return (false, $"Incorrect username or password. Attempt {_failedAttempts} of {MaxFailedAttempts}.");
    }

    public (bool Success, string Message) LoginAsStudent(int studentId)
    {
        Student? student = _studentService.GetById(studentId);
        if (student is null)
        {
            return (false, "No student found with that ID.");
        }

        Role = "Student";
        CurrentStudent = student;
        return (true, $"Logged in as {student.Name}.");
    }

    public void Logout()
    {
        Role = "None";
        CurrentStudent = null;
    }

    private static string Hash(string input)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}
