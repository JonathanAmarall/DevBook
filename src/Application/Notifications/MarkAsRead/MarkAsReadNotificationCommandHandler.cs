using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Application.Notifications.Get;
using Domain.Notifications;
using Domain.Repositories;
using SharedKernel;

namespace Application.Notifications.MarkAsRead;

internal sealed class MarkAsReadNotificationCommandHandler : ICommandHandler<MarkAsReadNotificationCommand, NotificationResponse>
{
    private readonly INotificationDeliveryRepository _deliveryRepository;
    private readonly IUserContext _userContext;
    public MarkAsReadNotificationCommandHandler(INotificationDeliveryRepository deliveryRepository, IUserContext userContext)
    {
        _deliveryRepository = deliveryRepository;
        _userContext = userContext;
    }

    public async Task<Result<NotificationResponse>> Handle(MarkAsReadNotificationCommand request, CancellationToken cancellationToken)
    {
        NotificationDelivery? notification = await _deliveryRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id && x.RecipientId == _userContext.UserId, cancellationToken);

        if (notification is null)
        {
            return Result.Failure<NotificationResponse>(NotificationErrors.NotFoundOrAlreadRead);
        }
        await _deliveryRepository.UnitOfWork.StartTransactionAsync(cancellationToken);

        notification.MarkAsRead();

        await _deliveryRepository.AddAsync(notification, cancellationToken);
        await _deliveryRepository.UnitOfWork.CommitChangesAsync(cancellationToken);

        return new NotificationResponse(
                notification.NotificationId,
                notification.SentOnUtc,
                notification.IsRead,
                notification.ReadOnUtc,
                notification.Status.ToString(),
                notification.Preview
        );
    }
}
