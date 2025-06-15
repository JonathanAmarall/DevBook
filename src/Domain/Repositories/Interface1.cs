using System.Linq.Expressions;
using Domain.Entries;
using Domain.Notifications;
using Domain.Users;

namespace Domain.Repositories;
public interface INotificationRespository : IRepository<Notification>
{
}

public interface INotificationScheduleRespository : IRepository<NotificationSchedule>
{
}


public interface IUserRespository : IRepository<User>
{
}

public interface IEntryRepository : IRepository<Entry>
{
    Task<long> CountDocumentsAsync(Expression<Func<Entry, bool>> query, CancellationToken cancellationToken);
}
