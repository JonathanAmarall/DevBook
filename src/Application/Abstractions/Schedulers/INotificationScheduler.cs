using Domain.Notifications;

namespace Application.Abstractions.Schedulers;
public interface INotificationScheduler
{
    Task ScheduleAsync(NotificationSchedule schedule, CancellationToken cancellationToken);
}
