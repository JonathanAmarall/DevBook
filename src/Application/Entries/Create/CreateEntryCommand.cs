using Application.Abstractions.Messaging;
using Application.Entries.GetById;
using Domain.Entries;

namespace Application.Entries.Create;
public class CreateEntryCommand : ICommand<EntryResponse>
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public EntryCategory Category { get; init; }
    public List<string> Tags { get; init; } = [];
    public string? ProjectName { get; init; } = string.Empty;
    public List<AttachmentLogEntryCommand> Attachments { get; init; } = [];
}

public record AttachmentLogEntryCommand
{
    public string FileName { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
}
