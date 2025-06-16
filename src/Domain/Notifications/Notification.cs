using SharedKernel;

namespace Domain.Notifications;
public sealed class Notification : Entity
{
    public string Title { get; private set; }
    public string EventId { get; private set; }
    public string UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public int Version { get; init; } = 1;
    public Dictionary<string, string> Metadata { get; private set; } = [];

    public Notification(string title, string eventId, string userId, NotificationType type)
    {
        Title = title;
        EventId = eventId;
        UserId = userId;
        Type = type;
    }
}
