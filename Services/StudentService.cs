using System.Text.Json;
using StudentRecordSystem.Models;

namespace StudentRecordSystem.Services;

/// <summary>
/// Central data store for student records. Registered as a Singleton so all
/// browser sessions share the same in-memory list, backed by a JSON data file
/// (Data/students.json) for persistence between application restarts.
/// This is the "database integration" layer for the application.
/// </summary>
public class StudentService
{
    private readonly List<Student> _students = new();
    private readonly string _filePath;
    private readonly object _lock = new();
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public StudentService(IWebHostEnvironment env)
    {
        string dataFolder = Path.Combine(env.ContentRootPath, "Data");
        Directory.CreateDirectory(dataFolder);
        _filePath = Path.Combine(dataFolder, "students.json");

        Load();

        if (_students.Count == 0)
        {
            SeedSampleData();
        }
    }

    public List<Student> GetAll()
    {
        lock (_lock)
        {
            return _students.OrderBy(s => s.Name).ToList();
        }
    }

    public Student? GetById(int id)
    {
        lock (_lock)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }
    }

    /// <summary>
    /// Searches/filters students by any combination of name, course code, and status.
    /// Any parameter left null or empty is ignored.
    /// </summary>
    public List<Student> Search(string? nameTerm, string? courseCode, StudentStatus? status)
    {
        lock (_lock)
        {
            IEnumerable<Student> query = _students;

            if (!string.IsNullOrWhiteSpace(nameTerm))
            {
                query = query.Where(s => s.Name.Contains(nameTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(courseCode))
            {
                query = query.Where(s => s.CourseCode == courseCode);
            }

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            return query.OrderBy(s => s.Name).ToList();
        }
    }

    public void Add(Student student)
    {
        lock (_lock)
        {
            if (student.Id <= 0)
            {
                student.Id = _students.Count == 0 ? 100000000 : _students.Max(s => s.Id) + 1;
            }
            _students.Add(student);
            Save();
        }
    }

    public void UpdateProfile(int id, string name, DateTime? dateOfBirth, string gender, string email,
        string phone, string address, string emergencyContact, string courseCode, StudentStatus status)
    {
        lock (_lock)
        {
            Student? student = _students.FirstOrDefault(s => s.Id == id);
            if (student is null)
            {
                return;
            }

            student.Name = name;
            student.DateOfBirth = dateOfBirth;
            student.Gender = gender;
            student.Email = email;
            student.Phone = phone;
            student.Address = address;
            student.EmergencyContact = emergencyContact;
            student.CourseCode = courseCode;
            student.Status = status;
            Save();
        }
    }

    /// <summary>
    /// Adds a new unit record, or updates the existing one if the student
    /// already has a record for the same unit code + semester.
    /// </summary>
    public void AddOrUpdateUnitRecord(int studentId, UnitRecord record)
    {
        lock (_lock)
        {
            Student? student = _students.FirstOrDefault(s => s.Id == studentId);
            if (student is null)
            {
                return;
            }

            UnitRecord? existing = student.UnitRecords
                .FirstOrDefault(u => u.UnitCode == record.UnitCode && u.Semester == record.Semester);

            if (existing is not null)
            {
                existing.UnitName = record.UnitName;
                existing.CreditPoints = record.CreditPoints;
                existing.Mark = record.Mark;
            }
            else
            {
                student.UnitRecords.Add(record);
            }

            Save();
        }
    }

    public void RemoveUnitRecord(int studentId, string unitCode, string semester)
    {
        lock (_lock)
        {
            Student? student = _students.FirstOrDefault(s => s.Id == studentId);
            if (student is null)
            {
                return;
            }

            UnitRecord? existing = student.UnitRecords
                .FirstOrDefault(u => u.UnitCode == unitCode && u.Semester == semester);

            if (existing is not null)
            {
                student.UnitRecords.Remove(existing);
                Save();
            }
        }
    }

    public void Remove(int id)
    {
        lock (_lock)
        {
            Student? student = _students.FirstOrDefault(s => s.Id == id);
            if (student is not null)
            {
                _students.Remove(student);
                Save();
            }
        }
    }

    private void Load()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                return;
            }

            string json = File.ReadAllText(_filePath);
            List<Student>? loaded = JsonSerializer.Deserialize<List<Student>>(json);

            if (loaded is not null)
            {
                _students.Clear();
                _students.AddRange(loaded);
            }
        }
        catch
        {
            // If the data file is missing or corrupt, start with an empty list
            // rather than crashing the application.
        }
    }

    private void Save()
    {
        try
        {
            string json = JsonSerializer.Serialize(_students, JsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch
        {
            // In a production system this failure would be logged.
        }
    }

    private void SeedSampleData()
    {
        Add(new Student
        {
            Id = 102345762,
            Name = "Vishwa Jayasinghe",
            DateOfBirth = new DateTime(2003, 5, 14),
            Gender = "Male",
            Email = "vishwa.j@student.ecu.edu.au",
            Phone = "0400 111 222",
            Address = "12 Example St, Joondalup WA",
            EmergencyContact = "Parent - 0400 999 888",
            CourseCode = "BCyberSec",
            Status = StudentStatus.Active,
            UnitRecords = new List<UnitRecord>
            {
                new() { UnitCode = "CSI2107", UnitName = "Malware Defence", Semester = "2026 Semester 1", CreditPoints = 15, Mark = 78 },
                new() { UnitCode = "CSI2201", UnitName = "Network Security", Semester = "2026 Semester 1", CreditPoints = 15, Mark = 82 },
                new() { UnitCode = "CSP3341", UnitName = "Programming Languages and Paradigms", Semester = "2026 Semester 2", CreditPoints = 15, Mark = null },
            }
        });

        Add(new Student
        {
            Id = 104829371,
            Name = "Nadeesha Silva",
            DateOfBirth = new DateTime(2002, 11, 2),
            Gender = "Female",
            Email = "nadeesha.s@student.ecu.edu.au",
            Phone = "0400 222 333",
            Address = "45 Sample Ave, Perth WA",
            EmergencyContact = "Parent - 0400 888 777",
            CourseCode = "BCompSci",
            Status = StudentStatus.Active,
            UnitRecords = new List<UnitRecord>
            {
                new() { UnitCode = "CSP2101", UnitName = "Data Structures", Semester = "2025 Semester 2", CreditPoints = 15, Mark = 92 },
                new() { UnitCode = "CSP1150", UnitName = "Introduction to Programming", Semester = "2025 Semester 1", CreditPoints = 15, Mark = 68 },
                new() { UnitCode = "CSP3341", UnitName = "Programming Languages and Paradigms", Semester = "2026 Semester 2", CreditPoints = 15, Mark = null },
            }
        });

        Add(new Student
        {
            Id = 108563940,
            Name = "Kasun Fernando",
            DateOfBirth = new DateTime(2003, 2, 20),
            Gender = "Male",
            Email = "kasun.f@student.ecu.edu.au",
            Phone = "0400 333 444",
            Address = "8 Demo Rd, Mandurah WA",
            EmergencyContact = "Sibling - 0400 777 666",
            CourseCode = "BSoftEng",
            Status = StudentStatus.OnLeave,
            UnitRecords = new List<UnitRecord>
            {
                new() { UnitCode = "CSP1150", UnitName = "Introduction to Programming", Semester = "2025 Semester 1", CreditPoints = 15, Mark = 65 },
                new() { UnitCode = "CSP2145", UnitName = "Object-Oriented Programming", Semester = "2025 Semester 2", CreditPoints = 15, Mark = 58 },
            }
        });

        Add(new Student
        {
            Id = 110238475,
            Name = "Emily Chen",
            DateOfBirth = new DateTime(2003, 8, 9),
            Gender = "Female",
            Email = "emily.c@student.ecu.edu.au",
            Phone = "0400 444 555",
            Address = "21 Coastal Rd, Joondalup WA",
            EmergencyContact = "Parent - 0400 666 555",
            CourseCode = "BCompSci",
            Status = StudentStatus.Active,
            UnitRecords = new List<UnitRecord>
            {
                new() { UnitCode = "CSP2101", UnitName = "Data Structures", Semester = "2025 Semester 2", CreditPoints = 15, Mark = 88 },
                new() { UnitCode = "CSP1150", UnitName = "Introduction to Programming", Semester = "2025 Semester 1", CreditPoints = 15, Mark = 79 },
                new() { UnitCode = "CSP3341", UnitName = "Programming Languages and Paradigms", Semester = "2026 Semester 2", CreditPoints = 15, Mark = null },
            }
        });

        Add(new Student
        {
            Id = 112904683,
            Name = "Liam O'Brien",
            DateOfBirth = new DateTime(2001, 12, 30),
            Gender = "Male",
            Email = "liam.o@student.ecu.edu.au",
            Phone = "0400 555 666",
            Address = "3 Harbour View, Fremantle WA",
            EmergencyContact = "Parent - 0400 222 111",
            CourseCode = "BCyberSec",
            Status = StudentStatus.Completed,
            UnitRecords = new List<UnitRecord>
            {
                new() { UnitCode = "CSI2107", UnitName = "Malware Defence", Semester = "2025 Semester 1", CreditPoints = 15, Mark = 91 },
                new() { UnitCode = "CSI2201", UnitName = "Network Security", Semester = "2025 Semester 1", CreditPoints = 15, Mark = 85 },
                new() { UnitCode = "CSP3341", UnitName = "Programming Languages and Paradigms", Semester = "2025 Semester 2", CreditPoints = 15, Mark = 88 },
            }
        });

        Add(new Student
        {
            Id = 115672198,
            Name = "Priya Nair",
            DateOfBirth = new DateTime(2004, 3, 17),
            Gender = "Female",
            Email = "priya.n@student.ecu.edu.au",
            Phone = "0400 666 777",
            Address = "60 Riverside Dr, Mandurah WA",
            EmergencyContact = "Parent - 0400 333 222",
            CourseCode = "BSoftEng",
            Status = StudentStatus.Withdrawn,
            UnitRecords = new List<UnitRecord>
            {
                new() { UnitCode = "CSP1150", UnitName = "Introduction to Programming", Semester = "2025 Semester 1", CreditPoints = 15, Mark = 55 },
            }
        });
    }
}
