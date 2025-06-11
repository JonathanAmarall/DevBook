using Application.Abstractions.Messaging;
using Application.Entries.GetById;

namespace Application.Entries.Complete;
public record MarkEntryAsResolvedCommand(string LogId) : ICommand<EntryResponse>;
