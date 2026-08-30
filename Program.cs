using StudentRecordSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services for dependency injection.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<StudentService>();   // shared student data store (JSON-backed)
builder.Services.AddSingleton<CourseService>();    // shared course reference list
builder.Services.AddSingleton<AuditLogService>();  // shared audit trail
builder.Services.AddScoped<AuthService>();         // per-browser-tab login/role state

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
