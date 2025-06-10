using SharedKernel;

namespace Domain.Notifications;

public static class NotificationErrors
{
    public static readonly Error AlreadySent = Error.Conflict(
        "Notification.AlreadySent",
        "Notification already sent");
}
