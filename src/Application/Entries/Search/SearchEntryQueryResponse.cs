using Domain.Entries;

namespace Application.Entries.Search;

public record SearchEntryQueryResponse
{
    public string Id { get; init; }
    public string Title { get; init; }
    public IEnumerable<string> Tags { get; init; }
    public EntryStatus Status { get; init; }
    public EntryCategory Category { get; init; }
    public DateTime CreatedOnUtc { get; init; }
};
