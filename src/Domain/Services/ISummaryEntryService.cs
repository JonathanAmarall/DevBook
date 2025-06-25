using System.ComponentModel;
using System.Text.Json.Serialization;
using SharedKernel;

namespace Domain.Services;
public interface ISummaryEntryService
{
    Task<Result<string>> CreateSummaryAsync(string userId, SummaryType summaryType, CancellationToken cancellationToken);
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SummaryType
{
    [Description("Not specified")]
    NotSpecified = 0,
    [Description("Today")]
    Today = 1,
    [Description("Yesterday")]
    Yesterday = 2,
    [Description("Weekly")]
    Weekly = 3,
    [Description("Monthly")]
    Monthly = 4
}

