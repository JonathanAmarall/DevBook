using Domain.Entries;
using Domain.Notifications;
using Domain.Users;
using MongoDB.Driver;

namespace Application.Abstractions.Data;
public interface IDatabaseContext
{
    IMongoCollection<Entry> LogEntries { get; }
    IMongoCollection<User> Users { get; }
    IMongoCollection<Notification> Notifications { get; }
    IMongoCollection<NotificationSchedule> NotificationSchedules { get; }
}
