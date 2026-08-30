namespace StudentRecordSystem.Services;

/// <summary>
/// Writes a simple timestamped audit trail of important actions (logins,
/// and admin add/edit/delete operations) to Data/audit.log. This satisfies
/// the assignment's "audit log of important changes" security feature.
/// </summary>
public class AuditLogService
{
    private readonly string _logPath;
    private readonly object _lock = new();

    public AuditLogService(IWebHostEnvironment env)
    {
        string dataFolder = Path.Combine(env.ContentRootPath, "Data");
        Directory.CreateDirectory(dataFolder);
        _logPath = Path.Combine(dataFolder, "audit.log");
    }

    public void Log(string action)
    {
        lock (_lock)
        {
            string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {action}{Environment.NewLine}";
            File.AppendAllText(_logPath, entry);
        }
    }

    public List<string> GetRecentEntries(int count = 20)
    {
        lock (_lock)
        {
            if (!File.Exists(_logPath))
            {
                return new List<string>();
            }

            return File.ReadAllLines(_logPath).Reverse().Take(count).ToList();
        }
    }
}
