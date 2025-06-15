using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Entries.Search;

internal sealed class SearchEntryQueryHandler(
    IEntryRepository entryRepository,
    IUserContext userContext) : IQueryHandler<SearchEntryQuery, PagedList<SearchEntryQueryResponse>>
{
    public async Task<Result<PagedList<SearchEntryQueryResponse>>> Handle(SearchEntryQuery request, CancellationToken cancellationToken)
    {
        PagedList<SearchEntryQueryResponse> pagedEntries = await entryRepository.PagedListAsync(
            x => x.UserId == userContext.UserId &&
                 (string.IsNullOrWhiteSpace(request.Title) || x.Title.Contains(request.Title, StringComparison.OrdinalIgnoreCase)) &&
                 (!request.Category.HasValue || x.Category == request.Category) &&
                 (!request.Status.HasValue || x.Status == request.Status),
            p => new SearchEntryQueryResponse
            {
                Category = p.Category,
                CreatedOnUtc = p.CreatedOnUtc,
                Id = p.Id,
                Status = p.Status,
                Tags = p.Tags,
                Title = p.Title
            },
            request.PageNumber.GetValueOrDefault(),
            request.PageSize.GetValueOrDefault(),
            cancellationToken: cancellationToken);

        return pagedEntries;
    }
}
