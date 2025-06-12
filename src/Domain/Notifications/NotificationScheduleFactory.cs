namespace Domain.Notifications;
public static class NotificationScheduleFactory
{
    private static readonly TimeOnly seventeenTimeToSent = new(17, 0);
    public static NotificationSchedule CreateDefaultReminder(Notification notification, TimeOnly? timeToSent = null)
    {
        timeToSent ??= seventeenTimeToSent;

        if (timeToSent < TimeOnly.MinValue || timeToSent > TimeOnly.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(timeToSent), "Time must be between 00:00 and 23:59.");
        }

        return new NotificationSchedule(
                notification.Id,
                NotificationFrequency.Daily,
                timeToSent,
                [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday]);
    }
}
