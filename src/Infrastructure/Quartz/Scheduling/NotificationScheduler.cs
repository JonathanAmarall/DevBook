using Application.Abstractions.Schedulers;
using Domain.Notifications;
using Quartz;

namespace Infrastructure.Quartz.Scheduling;
public class NotificationScheduler : INotificationScheduler
{
    private readonly ISchedulerFactory _factory;

    public NotificationScheduler(ISchedulerFactory factory)
    {
        _factory = factory;
    }

    public async Task ScheduleAsync(Notification notification)
    {
        IScheduler scheduler = await _factory.GetScheduler();
        (IJobDetail job, ITrigger trigger) = QuartzScheduleFactory.CreateJobFromNotification(notification);
        await scheduler.ScheduleJob(job, trigger);
    }
}
