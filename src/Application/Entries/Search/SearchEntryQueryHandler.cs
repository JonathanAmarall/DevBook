using System.Text.RegularExpressions;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entries;
using MongoDB.Bson;
using MongoDB.Driver;
using SharedKernel;

namespace Application.Entries.Search;

internal sealed class SearchEntryQueryHandler(
    IDatabaseContext databaseContext,
    IUserContext userContext) : IQueryHandler<SearchEntryQuery, PagedList<SearchEntryQueryResponse>>
{
    public async Task<Result<PagedList<SearchEntryQueryResponse>>> Handle(SearchEntryQuery request, CancellationToken cancellationToken)
    {
        var filters = new List<FilterDefinition<Entry>>
        {
            Builders<Entry>.Filter.Eq(x => x.UserId, userContext.UserId)
        };

        if (request.Category.HasValue)
        {
            filters.Add(Builders<Entry>.Filter.Eq(x => x.Category, request.Category));
        }

        if (request.Status.HasValue)
        {
            filters.Add(Builders<Entry>.Filter.Eq(x => x.Status, request.Status));
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            filters.Add(
                Builders<Entry>.Filter.Regex(
                    x => x.Title,
                    new BsonRegularExpression($".*{Regex.Escape(request.Title)}.*", "i")
                )
            );
        }

        FilterDefinition<Entry>? filter = Builders<Entry>.Filter.And(filters);
        long totalCount = await databaseContext.LogEntries.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        IEnumerable<SearchEntryQueryResponse> pagedData = await databaseContext.LogEntries.Find(filter)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Limit(request.PageSize)
            .Project<SearchEntryQueryResponse>(Builders<Entry>.Projection
                .Include(l => l.Id)
                .Include(l => l.Title)
                .Include(l => l.Status)
                .Include(l => l.Tags)
                .Include(l => l.Category)
                .Include(l => l.CreatedOnUtc))
            .SortByDescending(x => x.CreatedOnUtc)
            .ToListAsync(cancellationToken);

        return new PagedList<SearchEntryQueryResponse>(
            pagedData,
            request.PageNumber.GetValueOrDefault(),
            request.PageSize.GetValueOrDefault(),
            totalCount);
    }
}
