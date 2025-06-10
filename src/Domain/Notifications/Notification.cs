using SharedKernel;

namespace Domain.Notifications;
public sealed class Notification(string title, string eventId, string userId, string type) : Entity
{
    public string Title { get; private set; } = title;
    public string EventId { get; private set; } = eventId;
    public string UserId { get; private set; } = userId;
    public string Type { get; private set; } = type;
    public bool IsRead { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = [];
    public int Version { get; init; } = 1;

    public List<NotificationChannel> Channels { get; private set; } = [NotificationChannel.InApp];
    public NotificationFrequency Frequency { get; private set; } = NotificationFrequency.Once;
    public TimeOnly? ScheduledTime { get; private set; }
    public List<DayOfWeek> DaysOfWeek { get; private set; } = [];

    public void AddMetadata(Dictionary<string, string> metadata)
    {
        Metadata = metadata;
    }

    public void ConfigureSchedule(NotificationFrequency frequency, TimeOnly? time, List<DayOfWeek>? days = null)
    {
        Frequency = frequency;
        ScheduledTime = time;
        DaysOfWeek = frequency == NotificationFrequency.Weekly && days is not null
            ? days
            : [];
    }

    public void SetChannel(NotificationChannel channel)
    {
        if (Channels.Contains(channel))
        {
            return;
        }

        Channels.Add(channel);
    }

    public Result MarkAsRead()
    {
        if (IsRead)
        {
            return Result.Failure(NotificationErrors.AlreadySent);
        }

        IsRead = true;

        return Result.Success();
    }
}
