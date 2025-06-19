using Application.Abstractions.Schedulers;
using Domain.Notifications;
using Domain.Repositories;
using Domain.Users;
using SharedKernel;

namespace Application.Users.Common.Events;

internal sealed class UserRegisteredDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    private readonly INotificationScheduler _notificationScheduler;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationScheduleRepository _notificationScheduleRepository;

    public UserRegisteredDomainEventHandler(INotificationScheduler notificationScheduler, INotificationRepository notificationRepository, INotificationScheduleRepository notificationScheduleRepository, IUnitOfWork unitOfWork)
    {
        _notificationScheduler = notificationScheduler;
        _notificationRepository = notificationRepository;
        _notificationScheduleRepository = notificationScheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UserRegisteredDomainEvent @event, CancellationToken cancellationToken)
    {
        Notification notification = new(
            "Está na hora de adicionar uma nova entrada no seu diário!",
            "ReminderToAddEntry",
            @event.UserId,
            NotificationType.Reminder);

        var timeToSent = TimeOnly.FromDateTime(DateTime.Now.AddMinutes(1));

        NotificationSchedule schedule = NotificationScheduleFactory.CreateDailyReminder(notification, timeToSent);

        await _unitOfWork.StartTransactionAsync(cancellationToken);

        await _notificationRepository.AddAsync(
            notification,
            cancellationToken: cancellationToken
        );

        await _notificationScheduleRepository.AddAsync(
           schedule,
           cancellationToken: cancellationToken
       );

        await _unitOfWork.CommitChangesAsync(cancellationToken);

        await _notificationScheduler.ScheduleAsync(schedule);
    }
}
