using Application.Abstractions.Messaging;
using Domain.LogEntry;
using SharedKernel;

namespace Application.LogBook.Search;
public record SearchLogEntryQuery(
    string? Title,
    LogCategory? Category,
    List<string>? Tags,
    LogStatus? Status,
    DateTime? CreatedAt) : PagedRequest, IQuery<PagedList<SearchLogEntryQueryResponse>>;
