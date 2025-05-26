using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.LogBook.GetById;
using Domain.LogEntry;
using MongoDB.Driver;
using SharedKernel;

namespace Application.LogEntry.GetById;

internal sealed class GetLogEntryByIdQueryHandler(
    IDatabaseContext databaseContext,
    IUserContext userContext) : IQueryHandler<GetLogEntryByIdQuery, LogEntryResponse>
{
    public async Task<Result<LogEntryResponse>> Handle(GetLogEntryByIdQuery request, CancellationToken cancellationToken)
    {
        var filters = new List<FilterDefinition<Domain.LogEntry.LogEntry>>
        {
            Builders<Domain.LogEntry.LogEntry>.Filter.Eq(x => x.Id, request.Id),
            Builders<Domain.LogEntry.LogEntry>.Filter.Eq(x => x.UserId, userContext.UserId)
        };

        FilterDefinition<Domain.LogEntry.LogEntry>? filter = Builders<Domain.LogEntry.LogEntry>.Filter.And(filters);

        Domain.LogEntry.LogEntry logEntry = await databaseContext.LogEntries.Find(filter).FirstOrDefaultAsync(cancellationToken);

        return logEntry == null
            ? Result.Failure<LogEntryResponse>(LogEntryErrors.NotFound(request.Id))
            : new LogEntryResponse(logEntry);
    }
}
