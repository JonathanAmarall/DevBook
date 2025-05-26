using Application.Abstractions.Messaging;
using Application.LogEntry.GetById;
using Domain.LogEntry;

namespace Application.LogEntry.Create;
public class CreateLogEntryCommand : ICommand<LogEntryResponse>
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public LogCategory Category { get; init; }
    public List<string> Tags { get; init; } = [];
    public string? ProjectName { get; init; } = string.Empty;
    public List<AttachmentLogEntryCommand> Attachments { get; init; } = [];
}

public record AttachmentLogEntryCommand
{
    public string FileName { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
}
