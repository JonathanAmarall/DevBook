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

    public async Task ScheduleAsync(NotificationSchedule schedule)
    {
        IScheduler scheduler = await _factory.GetScheduler();
        (IJobDetail job, ITrigger trigger) = QuartzScheduleFactory.CreateJobFromNotificationSchedule(schedule);
        await scheduler.ScheduleJob(job, trigger);
    }
}
