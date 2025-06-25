using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Services;
using SharedKernel;

namespace Application.Summaries.Daily;

internal sealed class SummaryPeriodQueryHandler : IQueryHandler<SummaryPeriodQuery, SummaryPeriodQueryResponse>
{
    private readonly ISummaryEntryService _summaryEntryService;
    private readonly IUserContext _userContext;

    public SummaryPeriodQueryHandler(ISummaryEntryService summaryEntryService, IUserContext userContext)
    {
        _summaryEntryService = summaryEntryService;
        _userContext = userContext;
    }

    public async Task<Result<SummaryPeriodQueryResponse>> Handle(SummaryPeriodQuery request, CancellationToken cancellationToken)
    {
        Result<string> summary = await _summaryEntryService.CreateSummaryAsync(_userContext.UserId, request.SummaryType, cancellationToken);
        if (summary.IsFailure)
        {
            return Result.Failure<SummaryPeriodQueryResponse>(summary.Error);
        }

        return new SummaryPeriodQueryResponse(summary.Value);
    }
}
