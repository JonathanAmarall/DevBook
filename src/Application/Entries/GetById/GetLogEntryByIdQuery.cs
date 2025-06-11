using Application.Abstractions.Messaging;
using Application.Entries.GetById;

namespace Application.LogBook.GetById;
public record GetLogEntryByIdQuery(string Id) : IQuery<EntryResponse>;
