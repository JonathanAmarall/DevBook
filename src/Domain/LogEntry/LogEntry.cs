using SharedKernel;

namespace Domain.LogEntry;

public class LogEntry : Entity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new List<string>();
    public LogStatus Status { get; set; } = LogStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    // Referência ao usuário e projeto
    public string UserId { get; set; } = string.Empty;
    public string? ProjectId { get; set; }

    // Dados embutidos
    public List<Attachment> Attachments { get; set; } = new List<Attachment>();

    public void MarkAsResolved()
    {
        Status = LogStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;
    }
}
