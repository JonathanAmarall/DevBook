using System.Text.RegularExpressions;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.LogEntry;
using MongoDB.Bson;
using MongoDB.Driver;
using SharedKernel;

namespace Application.LogBook.Search;

internal sealed class SearchLogEntryQueryHandler(
    IDatabaseContext databaseContext,
    IUserContext userContext) : IQueryHandler<SearchLogEntryQuery, PagedList<SearchLogEntryQueryResponse>>
{
    public async Task<Result<PagedList<SearchLogEntryQueryResponse>>> Handle(SearchLogEntryQuery request, CancellationToken cancellationToken)
    {
        var filters = new List<FilterDefinition<LogEntry>>
        {
            Builders<LogEntry>.Filter.Eq(x => x.UserId, userContext.UserId)
        };

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            filters.Add(
                Builders<LogEntry>.Filter.Regex(
                    x => x.Title,
                    new BsonRegularExpression($".*{Regex.Escape(request.Title)}.*", "i")
                )
            );
        }

        FilterDefinition<LogEntry>? filter = Builders<LogEntry>.Filter.And(filters);
        long totalCount = await databaseContext.LogEntries.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        IEnumerable<SearchLogEntryQueryResponse> pagedData = await databaseContext.LogEntries.Find(filter)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Limit(request.PageSize)
            .Project<SearchLogEntryQueryResponse>(Builders<LogEntry>.Projection
                .Include(l => l.Id)
                .Include(l => l.Title)
                .Include(l => l.Status)
                .Include(l => l.Tags)
                .Include(l => l.CreatedAt)
                )
            .ToListAsync(cancellationToken);

        return new PagedList<SearchLogEntryQueryResponse>(
            pagedData,
            request.PageNumber.GetValueOrDefault(),
            request.PageSize.GetValueOrDefault(),
            totalCount);
    }
}
