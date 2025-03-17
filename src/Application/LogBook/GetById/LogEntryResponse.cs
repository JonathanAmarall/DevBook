using Domain.LogEntry;

namespace Application.LogBook.GetById;

public record LogEntryResponse
{
    public LogEntryResponse(LogEntry logEntry)
    {
        Id = logEntry.Id;
        Title = logEntry.Title;
        Description = logEntry.Description;
        Category = logEntry.Category;
        Tags = logEntry.Tags;
        Status = logEntry.Status.ToString();
        CreatedAt = logEntry.CreatedAt;
        ResolvedAt = logEntry.ResolvedAt;
    }

    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public List<string> Tags { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
