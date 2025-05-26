using Application.Abstractions.Messaging;
using Application.LogEntry.GetById;

namespace Application.LogBook.GetById;
public record GetLogEntryByIdQuery(string Id) : IQuery<LogEntryResponse>;
