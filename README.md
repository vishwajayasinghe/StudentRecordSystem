# Folio — Academic Records Registry (GUI — Blazor Server)

A browser-based C# web application for the CSP3341 assignments. It manages
full student academic records (personal details, course, current enrolment,
results, WAM/GPA, status), supports **Admin** and **Student** logins with
role-based access, and produces a printable academic transcript. Data is
persisted to a local JSON file.

## Features

| Area | What it does |
|---|---|
| **Login** | Separate Student (ID only) and Admin (username + SHA-256-hashed password, with lockout after 3 failed attempts) logins |
| **Student Dashboard** | Logged-in students see their own name, ID, course, status, WAM, GPA, current units, and recent results |
| **Personal details** | Name, date of birth, gender, email, phone, address, emergency contact |
| **Course information** | Course code, name, faculty, duration (seeded reference list) |
| **Current enrolment** | Units with no mark yet are treated as "currently enrolled" |
| **Academic results** | Units with a mark show their grade (HD/D/C/P/N) automatically |
| **WAM & GPA** | Calculated automatically, credit-point weighted, on a 4.0 GPA scale |
| **Student status** | Active / On Leave / Completed / Withdrawn / Suspended, shown as a coloured badge |
| **Academic transcript** | Full transcript view per student, with a Print/Export button |
| **Admin CRUD** | Add, edit, delete students; add/update/remove unit records and marks |
| **Search & filter** | Admins can search by name and filter by course and status |
| **Role-based access** | Students can only view their own record; only Admins can edit data |
| **Data file integration** | All data is stored in `Data/students.json` |
| **Input validation** | Required fields and email format validation on the student form |
| **Audit log** | Logins and every admin add/edit/delete are timestamped in `Data/audit.log` |

## Project structure
```
StudentRecordSystem/
├── Program.cs                     # App startup / DI configuration
├── App.razor                      # Blazor router
├── _Imports.razor                 # Global using statements
├── Pages/
│   ├── _Host.cshtml               # HTML page that hosts the Blazor app
│   ├── Index.razor                # Dashboard (student self-view / admin search)
│   ├── Login.razor                # Student ID login + Admin username/password login
│   ├── ManageStudents.razor       # Admin: add/edit/delete student profiles
│   ├── ManageResults.razor        # Admin: add/update/remove a student's unit records
│   └── Transcript.razor           # Full transcript view + print/export
├── Shared/
│   ├── MainLayout.razor
│   └── NavMenu.razor              # Role-aware sidebar navigation + logout
├── Models/
│   ├── Student.cs                 # Personal details, course, status, WAM/GPA calc
│   ├── Course.cs
│   ├── UnitRecord.cs              # A unit enrolment or completed result
│   ├── StudentStatus.cs           # Enum
│   └── GradeCalculator.cs         # Mark → grade → grade point
├── Services/
│   ├── StudentService.cs          # Data store + JSON persistence + search/filter
│   ├── CourseService.cs           # Seeded course reference list
│   ├── AuthService.cs             # Login, password hashing, lockout, role state
│   └── AuditLogService.cs         # Timestamped action log
├── wwwroot/css/app.css
├── Data/                          # students.json and audit.log created here at runtime
├── Properties/launchSettings.json
├── StudentRecordSystem.csproj
├── .gitignore
└── README.md
```

## Requirements
- [.NET SDK](https://dotnet.microsoft.com/download) — this project targets **.NET 10.0**.
  Check your installed version with `dotnet --list-runtimes`. If your installed
  runtime is different, change `<TargetFramework>net10.0</TargetFramework>` in
  `StudentRecordSystem.csproj` to match one of the versions listed
  (e.g. `net7.0`, `net8.0`).
- Visual Studio Code with the **C# Dev Kit** extension (recommended)

## How to run in VS Code

1. Open the folder `StudentRecordSystem` in VS Code.
2. Open a terminal (`` Ctrl+` ``) and run:
   ```
   dotnet restore
   dotnet run
   ```
3. Your browser should open automatically at `http://localhost:5080`.

## Trying it out

**As a student:** on the Login page, enter Student ID `102345762`
(sample data is seeded automatically) and click **Login as Student**. You'll
see that student's dashboard, with their own units, results, WAM, and GPA.
Click **View Full Transcript** to see the printable transcript.

**As an admin:** click **Administrator Login**, use username `admin` and
password `admin123`. You'll get the full student list with search/filter and
a WAM comparison chart. Click **Manage Students** to add/edit/delete a
profile, or **Results** next to any student to add or update their unit
enrolments and marks.

> ⚠️ The admin password is a hardcoded demo credential, intentionally simple
> for this assignment. A real system would never hardcode credentials in
> source code — it would use a proper identity/authentication service.

## Building for submission / GitHub

```
git init
git add .
git commit -m "Initial commit: Student Academic Record System"
git branch -M main
git remote add origin https://github.com/<your-username>/<your-repo-name>.git
git push -u origin main
```

## How this maps to the assignment's suggested features

| Suggested feature | Where it's implemented |
|---|---|
| Login (student + admin) | `Login.razor`, `AuthService` |
| Student dashboard | `Index.razor` (student view) |
| Personal details | `Student.cs`, `ManageStudents.razor` |
| Course information | `Course.cs`, `CourseService.cs` |
| Current unit enrolment | `UnitRecord.cs` (`Mark == null`), `Student.CurrentUnits` |
| Academic results | `UnitRecord.cs` (`Mark != null`), `Student.CompletedUnits` |
| WAM calculation | `Student.CalculateWAM()` |
| GPA calculation | `Student.CalculateGPA()`, `GradeCalculator.cs` |
| Student status | `StudentStatus.cs` enum, shown as a badge |
| Academic transcript | `Transcript.razor` |
| Admin CRUD | `ManageStudents.razor`, `ManageResults.razor` |
| Search/filter | `Index.razor` (admin view), `StudentService.Search()` |
| Role-based access | `AuthService.Role`, page-level `if (Auth.Role == ...)` guards |
| Database/data file integration | `StudentService` ↔ `Data/students.json` |
| Input validation | `DataAnnotationsValidator` in `ManageStudents.razor` |
| Password hashing | `AuthService.Hash()` (SHA-256) |
| Account lockout | `AuthService.IsLockedOut` (3 failed attempts) |
| Audit log | `AuditLogService` → `Data/audit.log` |
| Reports/export | `Transcript.razor` print button |

**Deliberately out of scope** (documented here for transparency, in case
you're asked about it): full multi-table relational database (SQLite/SQL
Server), real user account storage with salted hashes, session
timeout/expiry, and a fully editable Courses/Units admin screen. These are
reasonable extensions to mention in your report as "future work" without
being necessary to demonstrate the required language and design concepts.


