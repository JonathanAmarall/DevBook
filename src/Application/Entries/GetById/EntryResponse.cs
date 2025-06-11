using System.Text.Json.Serialization;
using Domain.Entries;

namespace Application.Entries.GetById;

public record EntryResponse
{
    public EntryResponse(Entry entry)
    {
        Id = entry.Id;
        Title = entry.Title;
        Description = entry.Description;
        Category = entry.Category.ToString();
        Tags = entry.Tags;
        Status = entry.Status.ToString();
        CreatedOnUtc = entry.CreatedOnUtc;
        ResolvedAt = entry.ResolvedAt;
    }

    [JsonConstructor]
    public EntryResponse()
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
