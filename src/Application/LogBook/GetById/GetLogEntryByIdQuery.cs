using Application.Abstractions.Messaging;

namespace Application.LogBook.GetById;
public record GetLogEntryByIdQuery(string Id) : IQuery<LogEntryResponse>;
