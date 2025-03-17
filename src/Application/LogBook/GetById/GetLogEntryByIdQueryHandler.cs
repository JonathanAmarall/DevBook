using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.LogEntry;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using SharedKernel;

namespace Application.LogBook.GetById;

internal sealed class GetLogEntryByIdQueryHandler(
    IDatabaseContext databaseContext,
    IUserContext userContext) : IQueryHandler<GetLogEntryByIdQuery, LogEntryResponse>
{
    public async Task<Result<LogEntryResponse>> Handle(GetLogEntryByIdQuery request, CancellationToken cancellationToken)
    {
        var filters = new List<FilterDefinition<LogEntry>>
        {
            Builders<LogEntry>.Filter.Eq(x => x.Id, request.Id),
            Builders<LogEntry>.Filter.Eq(x => x.UserId, userContext.UserId)
        };

        FilterDefinition<LogEntry>? filter = Builders<LogEntry>.Filter.And(filters);

        LogEntry logEntry = await databaseContext.LogEntries.Find(filter).FirstOrDefaultAsync(cancellationToken);

        if (logEntry == null)
        {
            return Result.Failure<LogEntryResponse>(
                new Error(nameof(StatusCodes.Status404NotFound),
                "Log entry not found.",
                ErrorType.NotFound));
        }

        return new LogEntryResponse(logEntry);
    }
}
