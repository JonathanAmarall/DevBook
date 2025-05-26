using System.Text.RegularExpressions;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.LogBook.Search;
using MongoDB.Bson;
using MongoDB.Driver;
using SharedKernel;

namespace Application.LogEntry.Search;

internal sealed class SearchLogEntryQueryHandler(
    IDatabaseContext databaseContext,
    IUserContext userContext) : IQueryHandler<SearchLogEntryQuery, PagedList<SearchLogEntryQueryResponse>>
{
    public async Task<Result<PagedList<SearchLogEntryQueryResponse>>> Handle(SearchLogEntryQuery request, CancellationToken cancellationToken)
    {
        var filters = new List<FilterDefinition<Domain.LogEntry.LogEntry>>
        {
            Builders<Domain.LogEntry.LogEntry>.Filter.Eq(x => x.UserId, userContext.UserId)
        };

        if (request.Category.HasValue)
        {
            filters.Add(Builders<Domain.LogEntry.LogEntry>.Filter.Eq(x => x.Category, request.Category));
        }

        if (request.Status.HasValue)
        {
            filters.Add(Builders<Domain.LogEntry.LogEntry>.Filter.Eq(x => x.Status, request.Status));
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            filters.Add(
                Builders<Domain.LogEntry.LogEntry>.Filter.Regex(
                    x => x.Title,
                    new BsonRegularExpression($".*{Regex.Escape(request.Title)}.*", "i")
                )
            );
        }

        FilterDefinition<Domain.LogEntry.LogEntry>? filter = Builders<Domain.LogEntry.LogEntry>.Filter.And(filters);
        long totalCount = await databaseContext.LogEntries.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        IEnumerable<SearchLogEntryQueryResponse> pagedData = await databaseContext.LogEntries.Find(filter)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Limit(request.PageSize)
            .Project<SearchLogEntryQueryResponse>(Builders<Domain.LogEntry.LogEntry>.Projection
                .Include(l => l.Id)
                .Include(l => l.Title)
                .Include(l => l.Status)
                .Include(l => l.Tags)
                .Include(l => l.Category)
                .Include(l => l.CreatedAt))
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return new PagedList<SearchLogEntryQueryResponse>(
            pagedData,
            request.PageNumber.GetValueOrDefault(),
            request.PageSize.GetValueOrDefault(),
            totalCount);
    }
}
