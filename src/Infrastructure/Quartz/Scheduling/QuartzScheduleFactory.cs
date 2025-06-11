using System.Globalization;
using Domain.Notifications;
using Infrastructure.Quartz.Jobs;
using Quartz;

namespace Infrastructure.Quartz.Scheduling;
public static class QuartzScheduleFactory
{
    public static (IJobDetail job, ITrigger trigger) CreateJobFromNotification(Notification notification)
    {
        IJobDetail job = JobBuilder.Create<SendNotificationJob>()
            .WithIdentity($"notification-{notification.Id}")
            .UsingJobData("NotificationId", notification.Id.ToString())
            .Build();

        TriggerBuilder triggerBuilder = TriggerBuilder.Create()
            .WithIdentity($"trigger-{notification.Id}");

        TimeOnly time = notification.ScheduledTime ?? new TimeOnly(9, 0);

        if (notification.Frequency == NotificationFrequency.Daily)
        {
            triggerBuilder.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(time.Hour, time.Minute));
        }
        else if (notification.Frequency == NotificationFrequency.Weekly && notification.DaysOfWeek.Any())
        {
            string days = string.Join(",", notification.DaysOfWeek.Select(d => d.ToString()[..3].ToUpper(CultureInfo.CurrentCulture)));
            string cron = $"0 {time.Minute} {time.Hour} ? * {days}";
            triggerBuilder.WithCronSchedule(cron);
        }
        else // Once or fallback
        {
            triggerBuilder.StartAt(DateBuilder.TodayAt(time.Hour, time.Minute, 0));
        }

        return (job, triggerBuilder.Build());
    }
}
