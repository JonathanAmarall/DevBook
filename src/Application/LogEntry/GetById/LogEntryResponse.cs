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
        CreatedOnUtc = logEntry.CreatedOnUtc;
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
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? ResolvedAt { get; init; }
}
