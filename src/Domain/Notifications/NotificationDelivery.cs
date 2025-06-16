using SharedKernel;

namespace Domain.Notifications;

public sealed class NotificationDelivery : Entity
{
    public string NotificationId { get; set; }
    public string RecipientId { get; set; }
    public NotificationChannel Channel { get; set; }
    public DateTime SentOnUtc { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadOnUtc { get; set; }
    public NotificationStatus Status { get; set; }
    public string? ErrorMessage { get; set; }

    public Result MarkAsSent()
    {
        SentOnUtc = DateTime.UtcNow;
        return Result.Success();
    }

    public Result MarkAsRead()
    {
        if (IsRead)
        {
            return Result.Failure(NotificationErrors.AlreadyRead);
        }

        IsRead = true;
        ReadOnUtc = DateTime.UtcNow;

        return Result.Success();
    }
}
