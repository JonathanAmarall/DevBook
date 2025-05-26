using System.Text.Json.Serialization;

namespace Application.LogEntry.GetById;

public record LogEntryResponse
{
    public LogEntryResponse(Domain.LogEntry.LogEntry logEntry)
    {
        Id = logEntry.Id;
        Title = logEntry.Title;
        Description = logEntry.Description;
        Category = logEntry.Category.ToString();
        Tags = logEntry.Tags;
        Status = logEntry.Status.ToString();
        CreatedAt = logEntry.CreatedAt;
        ResolvedAt = logEntry.ResolvedAt;
    }

    [JsonConstructor]
    public LogEntryResponse()
    {

    }

    public string Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string Category { get; init; }
    public List<string> Tags { get; init; }
    public string Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ResolvedAt { get; init; }
}
