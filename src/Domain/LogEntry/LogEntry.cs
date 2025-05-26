using SharedKernel;

namespace Domain.LogEntry;

public partial class LogEntry : Entity
{
    public LogEntry(string title, string description, LogCategory category, List<string> tags, string userId, string? projectName)
    {
        Id = Guid.NewGuid().ToString();
        Title = title;
        Description = description;
        Category = category;
        Tags = tags;
        UserId = userId;
        ProjectName = projectName;

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

    public string Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public LogCategory Category { get; private set; }
    public List<string> Tags { get; private set; }
    public LogStatus Status { get; private set; }
    public string UserId { get; private set; }
    public string? ProjectName { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; private set; }

    public void MarkAsResolved()
    {
        Status = LogStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
    }
}
