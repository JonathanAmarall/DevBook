using Application.Abstractions.Data;
using Domain.Notifications;
using Domain.Repositories;

namespace Infrastructure.Database.Repositories;

public sealed class NotificationDeliveryRepository : MongoRepository<NotificationDelivery>, INotificationDeliveryRepository
{
    public const string CollectionName = "NotificationsDelivery";

    public NotificationDeliveryRepository(IDatabaseContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    protected override string GetCollectionName() => CollectionName;
}

