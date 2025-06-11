using Application.Abstractions.Messaging;
using Domain.Entries;
using SharedKernel;

namespace Application.Entries.Search;
public record SearchLogEntryQuery(
    string? Title,
    EntryCategory? Category,
    List<string>? Tags,
    EntryStatus? Status,
    DateTime? CreatedAt) : PagedRequest, IQuery<PagedList<SearchLogEntryQueryResponse>>;
