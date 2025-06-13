using Application.Abstractions.Data;
using Application.Abstractions.Schedulers;
using Domain.Notifications;
using Domain.Users;
using SharedKernel;

namespace Application.Users.Common.Events;

internal sealed class ExternalUserRegisteredDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    private readonly INotificationScheduler _notificationScheduler;
    private readonly IDatabaseContext _databaseContext;

    public ExternalUserRegisteredDomainEventHandler(INotificationScheduler notificationScheduler, IDatabaseContext databaseContext)
    {
        _notificationScheduler = notificationScheduler;
        _databaseContext = databaseContext;
    }

    public async Task Handle(UserRegisteredDomainEvent @event, CancellationToken cancellationToken)
    {
        Notification notification = new(
            "Está na hora de adicionar uma nova entrada no seu diário!",
            "ReminderToAddEntry.",
            @event.UserId,
            NotificationType.Reminder);

        NotificationSchedule schedule = NotificationScheduleFactory.CreateDailyReminder(notification);

        await _databaseContext.Notifications.InsertOneAsync(
            notification,
            cancellationToken: cancellationToken
        );

        await _databaseContext.NotificationSchedules.InsertOneAsync(
           schedule,
           cancellationToken: cancellationToken
       );

        await _notificationScheduler.ScheduleAsync(schedule);
    }
}
