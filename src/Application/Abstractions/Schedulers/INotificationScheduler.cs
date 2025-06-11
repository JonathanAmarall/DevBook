using Domain.Notifications;

namespace Application.Abstractions.Schedulers;
public interface INotificationScheduler
{
    Task ScheduleAsync(Notification notification);
}
