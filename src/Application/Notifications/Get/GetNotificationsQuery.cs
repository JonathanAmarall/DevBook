using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Notifications.Get;
public sealed class GetNotificationsQuery : IQuery<PagedList<NotificationResponse>>
{
    public short PageSize { get; init; } = 10;
    public short PageNumber { get; init; } = 1;
}
