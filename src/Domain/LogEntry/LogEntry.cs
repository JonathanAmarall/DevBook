using SharedKernel;

namespace Domain.LogEntry;

public partial class LogEntry : Entity
{
    public LogEntry(string title, string description, LogCategory category, List<string> tags, string userId)
    {
        Title = title;
        Description = description;
        Category = category;
        Tags = tags;
        UserId = userId;

        if (Category == LogCategory.Bug || Category == LogCategory.Task)
        {
            Status = LogStatus.Open;
        }
        else
        {
            Status = LogStatus.Resolved;
            ResolvedAt = DateTime.UtcNow;
        }
    }

    public string Title { get; private set; }
    public string Description { get; private set; }
    public string UserId { get; private set; }
    public LogCategory Category { get; private set; }
    public List<string> Tags { get; private set; }
    public LogStatus Status { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    public void MarkAsResolved()
    {
        Status = LogStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
    }
}
