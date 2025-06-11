using Application.Abstractions.Schedulers;
using Domain.Notifications;
using Domain.Users;
using SharedKernel;

namespace Application.Users.Register;

internal sealed class ExternalUserRegisteredDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    private readonly INotificationScheduler _notificationScheduler;

    public ExternalUserRegisteredDomainEventHandler(INotificationScheduler notificationScheduler)
    {
        _notificationScheduler = notificationScheduler;
    }

    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        string userId = Guid.NewGuid().ToString("N");
        var notify = new Notification(
                "UserRegistered",
                $"User with ID {userId} has been registered.",
               userId,
                "UserRegisteredDomainEventHandler" + userId
            );

        notify.ConfigureSchedule(NotificationFrequency.Daily, new TimeOnly(8, 31, 0));
        // TODO: Send an email verification link, etc.
        await _notificationScheduler.ScheduleAsync(
            notify
        );
    }
}
