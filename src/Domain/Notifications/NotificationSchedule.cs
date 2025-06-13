using SharedKernel;

namespace Domain.Notifications;

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
