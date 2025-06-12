using SharedKernel;

namespace Domain.Notifications;
public sealed class Notification : Entity
{
    public string Title { get; private set; }
    public string EventId { get; private set; }
    public string UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadOnUtc { get; private set; }
    public bool IsSent { get; private set; }
    public DateTime? SentOnUtc { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = [];
    public int Version { get; init; } = 1;

    public Notification(string title, string eventId, string userId, NotificationType type)
    {
        Title = title;
        EventId = eventId;
        UserId = userId;
        Type = type;

        IsRead = false;
        IsSent = false;
    }

    public Result MarkAsSent()
    {
        if (IsSent)
        {
            return Result.Failure(NotificationErrors.AlreadySent);
        }

        IsSent = true;
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

public enum NotificationType
{
    Welcome = 0,
    Reminder = 1
}


public sealed class NotificationSchedule : Entity
{
    public string NotificationId { get; private set; }
    public NotificationFrequency Frequency { get; private set; }
    public TimeOnly? ScheduledTime { get; private set; }
    public List<DayOfWeek> DaysOfWeek { get; private set; } = [];
    public List<NotificationChannel> Channels { get; private set; } = [NotificationChannel.InApp];

    public NotificationSchedule(string notificationId, NotificationFrequency frequency, TimeOnly? scheduledTime, List<DayOfWeek>? days = null)
    {
        NotificationId = notificationId;
        Frequency = frequency;
        ScheduledTime = scheduledTime;
        DaysOfWeek = frequency == NotificationFrequency.Weekly && days is not null ? days : [];
    }

    public void SetChannel(NotificationChannel channel)
    {
        if (!Channels.Contains(channel))
        {
            Channels.Add(channel);
        }
    }
}
