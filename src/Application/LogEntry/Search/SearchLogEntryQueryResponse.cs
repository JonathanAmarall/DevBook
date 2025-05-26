using Domain.LogEntry;

namespace Application.LogBook.Search;

public record SearchLogEntryQueryResponse
{
    public string Id { get; init; }
    public string Title { get; init; }
    public string[] Tags { get; init; }
    public LogStatus Status { get; init; }
    public LogCategory Category { get; init; }
    public DateTime CreatedAt { get; init; }
};
