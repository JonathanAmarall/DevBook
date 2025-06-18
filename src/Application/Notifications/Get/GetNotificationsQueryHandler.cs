using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedKernel;

namespace Application.Notifications.Get;

internal sealed class GetNotificationsQueryHandler(
    INotificationDeliveryRepository deliveryRepository,
    IUserContext userContext)
    : IQueryHandler<GetNotificationsQuery, PagedList<NotificationResponse>>
{
    public async Task<Result<PagedList<NotificationResponse>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        PagedList<NotificationResponse> notifications = await deliveryRepository.PagedListAsync(
            n => n.RecipientId == userContext.UserId && n.SentOnUtc != null,
            n => new NotificationResponse(n.NotificationId, n.SentOnUtc, n.IsRead, n.ReadOnUtc, n.Status.ToString(), n.Preview),
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        return notifications;
    }
}
