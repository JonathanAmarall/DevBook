using Application.Abstractions.Data;
using Domain.Notifications;
using Domain.Repositories;

namespace Infrastructure.Database.Repositories;

public sealed class NotificationScheduleRepository : MongoRepository<NotificationSchedule>, INotificationScheduleRepository
{
    public const string CollectionName = "NotificationSchedules";

    public NotificationScheduleRepository(IDatabaseContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    protected override string GetCollectionName() => CollectionName;
}
