using Application.Abstractions.Schedulers;
using Domain.Notifications;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Quartz.Scheduling;
public class NotificationScheduler : INotificationScheduler
{
    private readonly ILogger<NotificationScheduler> _logger;
    private readonly ISchedulerFactory _factory;

    public NotificationScheduler(ISchedulerFactory factory, ILogger<NotificationScheduler> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task ScheduleAsync(NotificationSchedule schedule, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start - Scheduling notification with ID {NotificationId} with frequency {Frequency}.",
            schedule.NotificationId, schedule.Frequency);

        IScheduler scheduler = await _factory.GetScheduler(cancellationToken);
        (IJobDetail job, ITrigger trigger) = QuartzScheduleFactory.CreateJobFromNotificationSchedule(schedule);
        await scheduler.ScheduleJob(job, trigger, cancellationToken);

        _logger.LogInformation("End - Scheduled notification with ID {NotificationId} with frequency {Frequency}.",
            schedule.NotificationId, schedule.Frequency);
    }
}
