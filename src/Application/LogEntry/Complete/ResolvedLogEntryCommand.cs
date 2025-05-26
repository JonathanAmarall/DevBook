using Application.Abstractions.Messaging;
using Application.LogEntry.GetById;

namespace Application.LogEntry.Complete;
public record ResolvedLogEntryCommand(string LogId) : ICommand<LogEntryResponse>;
