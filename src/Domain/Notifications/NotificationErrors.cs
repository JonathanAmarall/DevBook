using SharedKernel;

namespace Domain.Notifications;

public static class NotificationErrors
{
    public static readonly Error AlreadySent = Error.Conflict(
        "Notification.AlreadySent",
        "Notification already sent");

    public static readonly Error AlreadyRead = Error.Conflict(
        "Notification.AlreadyRead",
        "Notification already read");
}
