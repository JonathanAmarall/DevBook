using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.LogEntry;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using SharedKernel;

namespace Application.LogBook.GetById;
public record GetLogEntryByIdQuery(string Id) : IQuery<LogEntryResponse>;

public record LogEntryResponse
{
    public LogEntryResponse(LogEntry logEntry)
    {
        Id = logEntry.Id;
        Title = logEntry.Title;
        Description = logEntry.Description;
        Category = logEntry.Category;
        Tags = logEntry.Tags;
        Status = logEntry.Status.ToString();
        CreatedAt = logEntry.CreatedAt;
        ResolvedAt = logEntry.ResolvedAt;
    }

    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public List<string> Tags { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

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

