using Application.Abstractions.Messaging;
using Domain.Services;

namespace Application.Summaries.Daily;
public record SummaryPeriodQuery : IQuery<SummaryPeriodQueryResponse>
{
    public SummaryType SummaryType { get; init; }
}
