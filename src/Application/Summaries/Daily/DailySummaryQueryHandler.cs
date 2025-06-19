using System.Text.Json;
using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Caching.Distributed;
using SharedKernel;

namespace Application.Summaries.Daily;

internal sealed class DailySummaryQueryHandler : IQueryHandler<DailySummaryQuery, DailySummaryResponse>
{
    private readonly IUserContext _userContext;
    private readonly IEntryRepository _entryRepository;
    private readonly ITextSummaryGeneratorService _textSummaryGeneratorService;
    private readonly IDistributedCache _cache;

    public DailySummaryQueryHandler(IEntryRepository entryRepository, ITextSummaryGeneratorService textSummaryGeneratorService, IUserContext userContext, IDistributedCache cache)
    {
        _entryRepository = entryRepository;
        _textSummaryGeneratorService = textSummaryGeneratorService;
        _userContext = userContext;
        _cache = cache;
    }

    public async Task<Result<DailySummaryResponse>> Handle(DailySummaryQuery request, CancellationToken cancellationToken)
    {
        var dailyEntries = await _entryRepository.FilterAsync(
            entry => entry.UserId == _userContext.UserId && entry.CreatedOnUtc.Date == DateTime.UtcNow.Date,
            x => new { x.Title, x.Description },
            cancellationToken: cancellationToken
        );

        string cacheKey = $"SummaryGenerator:{_userContext.UserId}:{DateTime.UtcNow.Date}:EntriesCount:{dailyEntries.Count()}";
        byte[]? jsonFromCache = await _cache.GetAsync(cacheKey, cancellationToken);
        if (jsonFromCache != null)
        {
            return JsonSerializer.Deserialize<DailySummaryResponse>(jsonFromCache);
        }

        string textToSummary = string.Join(Environment.NewLine, dailyEntries.Select(x => x.Title + " " + x.Description));
        if (string.IsNullOrWhiteSpace(textToSummary))
        {
            return new DailySummaryResponse
            {
                SummaryText = "No entries found for today."
            };
        }

        string? summaryResponse = await _textSummaryGeneratorService.GenerateTextSummaryAsync(
           "Resuma os seguintes textos de forma impessoal, de forma direta, simples e em no máximo 100 palavras: " + textToSummary, cancellationToken);

        var dailySummaryResponse = new DailySummaryResponse
        {
            SummaryText = summaryResponse
        };

        await _cache.SetAsync(cacheKey, JsonSerializer.SerializeToUtf8Bytes(dailySummaryResponse), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        }, cancellationToken);

        return dailySummaryResponse;
    }
}
