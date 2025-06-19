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

    public static readonly Error NotFoundOrAlreadRead = Error.Conflict(
        "Notification.NotFoundOrAlreadRead",
        "Notification not found or you do not have permission to access it.");
}
