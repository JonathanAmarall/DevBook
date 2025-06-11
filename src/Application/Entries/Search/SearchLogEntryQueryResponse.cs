using Domain.Entries;

namespace Application.Entries.Search;

public record SearchLogEntryQueryResponse
{
    public string Id { get; init; }
    public string Title { get; init; }
    public string[] Tags { get; init; }
    public EntryStatus Status { get; init; }
    public EntryCategory Category { get; init; }
    public DateTime CreatedAt { get; init; }
};
