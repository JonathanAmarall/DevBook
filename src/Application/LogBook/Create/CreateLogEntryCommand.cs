using Application.Abstractions.Messaging;

namespace Application.LogBook.Create;
public class CreateLogEntryCommand : ICommand<CreateLogEntryCommandResponse>
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public List<string> Tags { get; init; } = [];
    public string? ProjectId { get; init; }
    public List<AttachmentLogEntryCommand> Attachments { get; init; } = [];
}

public record AttachmentLogEntryCommand
{
    public string FileName { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
}
