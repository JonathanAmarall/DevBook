using Application.Abstractions.Messaging;

namespace Application.Entries.GetById;
public record GetEntryByIdQuery(string Id) : IQuery<EntryResponse>;
