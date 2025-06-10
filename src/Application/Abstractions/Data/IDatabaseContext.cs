using Domain.Notifications;
using Domain.Users;
using MongoDB.Driver;

namespace Application.Abstractions.Data;
public interface IDatabaseContext
{
    IMongoCollection<Domain.LogEntry.LogEntry> LogEntries { get; }
    IMongoCollection<User> Users { get; }
    IMongoCollection<Notification> Notifications { get; }
}
