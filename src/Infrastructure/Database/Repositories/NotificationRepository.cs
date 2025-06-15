using Application.Abstractions.Data;
using Domain.Notifications;
using Domain.Repositories;

namespace Infrastructure.Database.Repositories;

public sealed class NotificationRepository : MongoRepository<Notification>, INotificationRespository
{
    public const string CollectionName = "Notifications";

    public NotificationRepository(IDatabaseContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    protected override string GetCollectionName() => CollectionName;
}
