using Application.Abstractions.Schedulers;
using Domain.Notifications;
using Domain.Repositories;
using Domain.Users;
using SharedKernel;

namespace Application.Users.Common.Events;

internal sealed class ExternalUserRegisteredDomainEventHandler : IDomainEventHandler<UserRegisteredDomainEvent>
{
    private readonly INotificationScheduler _notificationScheduler;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationRespository _notificationRepository;
    private readonly INotificationScheduleRespository _notificationScheduleRepository;

    public ExternalUserRegisteredDomainEventHandler(INotificationScheduler notificationScheduler, INotificationRespository notificationRepository, INotificationScheduleRespository notificationScheduleRepository, IUnitOfWork unitOfWork)
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

        NotificationSchedule schedule = NotificationScheduleFactory.CreateDailyReminder(notification);

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

        await _notificationScheduler.ScheduleAsync(schedule, cancellationToken);
    }
}
