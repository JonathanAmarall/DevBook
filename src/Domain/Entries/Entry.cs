using SharedKernel;

namespace Domain.Entries;

public partial class Entry : Entity
{
    public Entry(string title, string description, EntryCategory category, List<string> tags, string userId)
    {
        Title = title;
        Description = description;
        Category = category;
        Tags = tags;
        UserId = userId;

        if (Category == EntryCategory.Bug || Category == EntryCategory.Task)
        {
            Status = EntryStatus.Open;
        }
        else
        {
            Status = EntryStatus.Resolved;
            ResolvedAt = DateTime.UtcNow;
        }
    }

    public string Title { get; private set; }
    public string Description { get; private set; }
    public string UserId { get; private set; }
    public EntryCategory Category { get; private set; }
    public List<string> Tags { get; private set; }
    public EntryStatus Status { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    public void MarkAsResolved()
    {
        Status = EntryStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
    }
}
