namespace Domain.LogEntry;

public class Attachment
{
    public string FileName { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public DateTime UploadedAt { get; init; } = DateTime.UtcNow;
}

