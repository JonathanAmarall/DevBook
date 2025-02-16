using Application.Abstractions.Messaging;

namespace Application.LogBook.Create;
public class CreateLogEntryCommand : ICommand<CreateLogEntryCommandResponse>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public string? ProjectId { get; set; }
    public List<AttachmentLogEntryCommand> Attachments { get; set; } = [];
}

public record AttachmentLogEntryCommand
{
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
