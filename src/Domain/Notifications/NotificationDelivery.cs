using SharedKernel;

namespace Domain.Notifications;

public sealed class NotificationDelivery : Entity
{
    public NotificationDelivery(string notificationId, string recipientId, NotificationChannel channel, string preview)
    {
        NotificationId = notificationId;
        RecipientId = recipientId;
        Channel = channel;
        Preview = preview;
        IsRead = false;
    }

    public string NotificationId { get; private set; }
    public string RecipientId { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public DateTime? SentOnUtc { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadOnUtc { get; private set; }
    public NotificationStatus Status { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string Preview { get; private set; }

    public void SetErrorMessage(string error)
    {
        ErrorMessage = error;
    }

    public Result MarkAsSent()
    {
        Status = NotificationStatus.Sent;
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
