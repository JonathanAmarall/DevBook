namespace Domain.Notifications;
public static class NotificationFactory
{
    public static Notification CreateDefaultReminderEntries(string userId)
    {
        Notification defaultNotification = new(
             "Default reminder to fill in the diary",
             Guid.NewGuid().ToString(),
             userId,
             "DefaultReminderToFillDiary");

        defaultNotification.ConfigureSchedule(
            NotificationFrequency.Daily,
            new TimeOnly(17, 0),
            [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday]);

        return defaultNotification;
    }
}
