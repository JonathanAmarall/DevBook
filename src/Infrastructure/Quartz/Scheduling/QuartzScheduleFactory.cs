using System.Globalization;
using Domain.Notifications;
using Infrastructure.Quartz.Jobs;
using Quartz;

namespace Infrastructure.Quartz.Scheduling;
public static class QuartzScheduleFactory
{
    public static (IJobDetail job, ITrigger trigger) CreateJobFromNotificationSchedule(NotificationSchedule schedule)
    {
        IJobDetail job = JobBuilder.Create<SendNotificationJob>()
            .WithIdentity($"notification-{schedule.NotificationId}")
            .UsingJobData("NotificationId", schedule.NotificationId.ToString())
            .Build();

        TriggerBuilder triggerBuilder = TriggerBuilder.Create()
            .WithIdentity($"trigger-{schedule.NotificationId}");

        TimeOnly time = schedule.ScheduledTime ?? new TimeOnly(9, 0);

        if (schedule.Frequency == NotificationFrequency.Daily)
        {
            triggerBuilder.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(time.Hour, time.Minute));
        }
        else if (schedule.Frequency == NotificationFrequency.Weekly && schedule.DaysOfWeek.Any())
        {
            string days = string.Join(",", schedule.DaysOfWeek.Select(d => d.ToString()[..3].ToUpper(CultureInfo.CurrentCulture)));
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
