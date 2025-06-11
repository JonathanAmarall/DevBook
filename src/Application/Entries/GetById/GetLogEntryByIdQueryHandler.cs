using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.LogBook.GetById;
using Domain.Entries;
using MongoDB.Driver;
using SharedKernel;

namespace Application.Entries.GetById;

internal sealed class GetLogEntryByIdQueryHandler(
    IDatabaseContext databaseContext,
    IUserContext userContext) : IQueryHandler<GetLogEntryByIdQuery, EntryResponse>
{
    public async Task<Result<EntryResponse>> Handle(GetLogEntryByIdQuery request, CancellationToken cancellationToken)
    {
        var filters = new List<FilterDefinition<Entry>>
        {
            Builders<Entry>.Filter.Eq(x => x.Id, request.Id),
            Builders<Entry>.Filter.Eq(x => x.UserId, userContext.UserId)
        };

        FilterDefinition<Entry>? filter = Builders<Entry>.Filter.And(filters);

        Entry logEntry = await databaseContext.LogEntries.Find(filter).FirstOrDefaultAsync(cancellationToken);

        return logEntry == null
            ? Result.Failure<EntryResponse>(EntryErrors.NotFound(request.Id))
            : new EntryResponse(logEntry);
    }
}
