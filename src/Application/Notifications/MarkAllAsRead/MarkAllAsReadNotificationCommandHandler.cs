using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Notifications;
using Domain.Repositories;
using SharedKernel;

namespace Application.Notifications.MarkAsRead;

internal sealed class MarkAllAsReadNotificationCommandHandler : ICommandHandler<MarkAllAsReadNotificationCommand>
{
    private readonly INotificationDeliveryRepository _deliveryRepository;
    private readonly IUserContext _userContext;
    public MarkAllAsReadNotificationCommandHandler(INotificationDeliveryRepository deliveryRepository, IUserContext userContext)
    {
        _deliveryRepository = deliveryRepository;
        _userContext = userContext;
    }

    public async Task<Result> Handle(MarkAllAsReadNotificationCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<NotificationDelivery>? notifications = await _deliveryRepository.FilterAsync(
            x => !x.IsRead && x.RecipientId == _userContext.UserId, cancellationToken);

        if (!notifications.Any())
        {
            return Result.Failure(NotificationErrors.NotFoundOrAlreadRead);
        }

        await _deliveryRepository.UnitOfWork.StartTransactionAsync(cancellationToken);

        foreach (NotificationDelivery notification in notifications)
        {
            notification.MarkAsRead();
            await _deliveryRepository.AddAsync(notification, cancellationToken);
        }

        await _deliveryRepository.UnitOfWork.CommitChangesAsync(cancellationToken);

        return Result.Success();
    }
}
