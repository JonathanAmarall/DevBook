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
}
