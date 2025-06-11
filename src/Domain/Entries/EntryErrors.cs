using SharedKernel;

namespace Domain.Entries;
public static class EntryErrors
{
    public static Error NotFound(string logId) => Error.NotFound(
    "LogEntry.NotFound",
    $"The Log with the Id = '{logId}' was not found");
}
