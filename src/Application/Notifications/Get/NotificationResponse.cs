namespace Application.Notifications.Get;

public record NotificationResponse(
string Id,
DateTime? SentOnUtc,
bool IsRead,
DateTime? ReadOnUtc,
string Status,
string Preview);
