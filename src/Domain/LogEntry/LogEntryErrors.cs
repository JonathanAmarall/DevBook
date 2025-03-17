using SharedKernel;

namespace Domain.LogEntry;
public static class LogEntryErrors
{
    public static Error NotFound(string logId) => Error.NotFound(
    "LogEntry.NotFound",
    $"The Log with the Id = '{logId}' was not found");

}
